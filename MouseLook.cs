using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Ladder.Scripts
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 20f;
        public float YSensitivity = 20f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool xboxController;

        private string dynamicInputX;
        private string dynamicInputY;

        private float rotateToSmoothTime = 8f;

        private Quaternion m_CharacterTargetRot;

        private Quaternion m_CameraTargetRot;

        public void SwitchInputs()
        {
            dynamicInputX = xboxController ? "Paneo" : "Mouse X";
            dynamicInputY = xboxController ? "Tildeo" : "Mouse Y";
        }

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public void LookRotation(Transform character, Transform camera, Transform ladderTrigger)
        {
            if (ladderTrigger == null)
            {
                float yRot = CrossPlatformInputManager.GetAxis(dynamicInputX) * XSensitivity;
                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            }
            else
            {
                Vector3 direction = ladderTrigger.forward;
                direction.y = 0;

                Quaternion lookRotation = Quaternion.LookRotation(direction);

                m_CharacterTargetRot = lookRotation;

            }

            float xRot = CrossPlatformInputManager.GetAxis(dynamicInputY) * YSensitivity;

            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (ladderTrigger != null)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
            rotateToSmoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    rotateToSmoothTime * Time.deltaTime);
            }
            else if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }


        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
