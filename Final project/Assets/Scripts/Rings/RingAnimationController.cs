using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class RingAnimationController : MonoBehaviour {
    Animator[] animators;
    int currentAnimator;
    [SerializeField] AnimationClip[] animationClips;

    void Awake() {
        animators = GetComponentsInChildren<Animator>();
    }

    // AimRingAtEnemy will be called when a ring that requires a target is cast
    public bool AimRingAtEnemy(GameObject target) {
        if(target == null) {
            return false;
        }
        Vector3 direction = target.transform.position - transform.position; // Get the difference between player and target position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // -180 to 180 degrees

        transform.rotation = Quaternion.Euler(0, 0, angle);
        return true;
    }

    public void PlayAnimation(string animationTrigger) {
        if(animationTrigger == null) {
            Debug.LogError("Animation ID is -1");
            return;
        }

        if(currentAnimator >= animators.Count()) {
            currentAnimator = 0;
        }
        
        Animator targetAnimator = animators[currentAnimator];
        currentAnimator++;
        targetAnimator.SetTrigger(animationTrigger);
        // AnimationClip targetClip = animationClips[animationID+1];

        // PlayableGraph graph = PlayableGraph.Create();
        // AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Animation", targetAnimator);
        // AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, targetClip);

        // output.SetSourcePlayable(clipPlayable);
        // graph.Play();
    }
}
