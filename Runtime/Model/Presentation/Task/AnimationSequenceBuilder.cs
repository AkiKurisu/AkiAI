using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
namespace Kurisu.AkiAI.Playables
{
    /// <summary>
    /// Provide an animation sequence using UnityEngine.Playables API
    /// </summary>
    public class AnimationSequenceBuilder : IDisposable
    {
        private readonly PlayableGraph playableGraph;
        private AnimationPlayableOutput playableOutput;
        private readonly List<ITask> taskBuffer = new();
        private Playable rootMixer;
        private Playable mixerPointer;
        private SequenceTask sequence;
        private float fadeOutTime = 0f;
        public AnimationSequenceBuilder(Animator animator)
        {
            playableGraph = PlayableGraph.Create($"{animator.name}_sequence");
            playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
            mixerPointer = rootMixer = AnimationMixerPlayable.Create(playableGraph, 2);
            playableOutput.SetSourcePlayable(mixerPointer);
        }
        public AnimationSequenceBuilder Append(AnimationClip animationClip, float fadeIn)
        {
            if (IsBuilt()) return this;
            if (!IsValid()) return this;
            var duration = animationClip.length;
            var clipPlayable = AnimationClipPlayable.Create(playableGraph, animationClip);
            clipPlayable.SetDuration(duration);
            clipPlayable.SetSpeed(0d);
            if (mixerPointer.GetInput(1).IsNull())
            {
                playableGraph.Connect(clipPlayable, 0, mixerPointer, 1);
            }
            else
            {
                var newMixer = AnimationMixerPlayable.Create(playableGraph, 2);
                var right = mixerPointer.GetInput(1);
                taskBuffer.Add(new WaitPlayableTask(right, duration - fadeIn));
                //Disconnect leaf
                playableGraph.Disconnect(mixerPointer, 1);
                //Right=>left
                playableGraph.Connect(right, 0, newMixer, 0);
                //New right leaf
                playableGraph.Connect(clipPlayable, 0, newMixer, 1);
                //Connect to parent
                playableGraph.Connect(newMixer, 0, mixerPointer, 1);
                //Update pointer
                mixerPointer = newMixer;
            }
            mixerPointer.SetInputWeight(0, 1);
            mixerPointer.SetInputWeight(1, 0);
            taskBuffer.Add(new FadeInTask(mixerPointer, clipPlayable, fadeIn));
            return this;
        }
        /// <summary>
        /// Build animation sequence
        /// </summary>
        public void Build()
        {
            if (IsBuilt())
            {
                Debug.LogWarning("Graph is already built, rebuild is not allowed");
                return;
            }
            if (!IsValid())
            {
                Debug.LogWarning("Graph is already destroyed before build");
                return;
            }
            BuildInternal(new SequenceTask(Dispose));
        }
        /// <summary>
        /// Append animation sequence after an existed sequence
        /// </summary>
        /// <param name="sequenceTask"></param>
        public void Build(SequenceTask sequenceTask)
        {
            if (IsBuilt())
            {
                Debug.LogWarning("Graph is already built, rebuild is not allowed");
                return;
            }
            if (!IsValid())
            {
                Debug.LogWarning("Graph is already destroyed before build");
                return;
            }
            BuildInternal(sequenceTask);
            sequenceTask.Append(new CallBackTask(Dispose));
        }
        private void BuildInternal(SequenceTask sequenceTask)
        {
            if (!playableGraph.IsPlaying())
            {
                playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
                playableGraph.Play();
            }
            foreach (var task in taskBuffer)
                sequenceTask.Append(task);
            var right = (AnimationClipPlayable)mixerPointer.GetInput(1);
            sequenceTask.Append(new WaitPlayableTask(right, right.GetAnimationClip().length - fadeOutTime));
            if (fadeOutTime > 0)
            {
                sequenceTask.Append(new FadeOutTask(rootMixer, right, fadeOutTime));
            }
            sequence = sequenceTask;
            taskBuffer.Clear();
        }
        public AnimationSequenceBuilder SetFadeOut(float fadeOut)
        {
            if (IsBuilt()) return this;
            fadeOutTime = fadeOut;
            return this;
        }
        public bool IsBuilt()
        {
            return sequence != null;
        }
        public bool IsValid()
        {
            return playableGraph.IsValid();
        }
        public void Dispose()
        {
            sequence?.Abort();
            if (playableGraph.IsValid())
            {
                playableGraph.Destroy();
            }
        }
    }
}
