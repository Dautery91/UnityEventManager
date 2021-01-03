using UnityEngine;
using HitTheStuff.CharacterState;
using System;
using DarkTonic.MasterAudio;
using DrakesGames.UnityCore.Animation;
using HitTheStuff.Combat;

namespace HitTheStuff.Events
{
    /// <summary>
    /// Class that feeds events coming from animator / animation clips into the local event manager
    /// </summary>
    public class AnimationEventManager : MonoBehaviour, ICharacterStateReciever
    {
        [SerializeField] private GOLocalEventManager localEM = null;
        private CharacterStateManager characterStateManager = null;

        #region Unity Methods


        private void PlaySFXGroup(string soundGroup)
        {
            MasterAudio.PlaySoundAndForget(soundGroup);
        }

        private void OnEnable()
        {
            OnSpawned();
        }

        void OnSpawned()
        {
            if (localEM == null)
            {
                localEM = GetComponentInParent<GOLocalEventManager>();
            }

            SceneLinkedSMB<AnimationEventManager>.Initialise(GetComponent<Animator>(), this);
        }

        #endregion

        public void EnableWeaponCollider()
        {
            //Debug.Break();
            LocalHitBoxAnimationStartEventInfo eventInfo = new LocalHitBoxAnimationStartEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        public void DisableWeaponCollider()
        {
            LocalHitBoxAnimationEndEventInfo eventInfo = new LocalHitBoxAnimationEndEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        // Maybe turn this into a more generic visual FX event?  Could be modeled similar to the sound thing
        private void OnSweetSpotStart()
        {
            LocalSweetSpotStartEventInfo eventInfo = new LocalSweetSpotStartEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        private void OnSweetSpotEnd()
        {
            LocalSweetSpotEndEventInfo eventInfo = new LocalSweetSpotEndEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        private void ChangeCharacterActiveAction(ActiveAction action)
        {
            characterStateManager.CurrentActiveAction = action;
        }

        private void OnReadyProjectile()
        {
            LocalReadyProjectileEventInfo eventInfo = new LocalReadyProjectileEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        private void OnThrowProjectile()
        {
            LocalThrowProjectileEventInfo eventInfo = new LocalThrowProjectileEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        private void OnJumpTakeOff()
        {
            LocalOnJumpTakeOffEventInfo eventInfo = new LocalOnJumpTakeOffEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        private void OnSlideForce()
        {
            LocalSlideForceStartEventInfo eventInfo = new LocalSlideForceStartEventInfo();
            localEM.FireLocalEvent(eventInfo);
        }

        public void InitializeCharacterState(CharacterStateManager characterStateManager)
        {
            this.characterStateManager = characterStateManager;
        }

        private enum AnimationEventsEnum
        { 
            EnableWeapon,
            DisableWeapon,
            SSStart,
            SSEnd,
            StartParryWindow,
            JumpTakeoff
        }
    }
}
