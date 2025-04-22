using UnityEngine;
using UnityEditor;


namespace Julian.RBController.Editor
{
    [CustomEditor(typeof(RigidbodyController))]
    public class RigidbodyControllerEditor : UnityEditor.Editor
    {
        private RigidbodyController _controller;
        private bool _showDebugData = true;
        private bool _showPhysicsData = true;
        private bool _showMovementData = true;
        private bool _showGroundCheckerData = true;
        private bool _showHeightSpringData = true;
        private bool _showCameraData = true;
        private bool _showStateData = true;

        private void OnEnable()
        {
            _controller = (RigidbodyController)target;
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Only show debug data in play mode
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Debug data will be displayed during play mode.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Data", EditorStyles.boldLabel);

            _showDebugData = EditorGUILayout.Foldout(_showDebugData, "Debug Data", true);
            if (_showDebugData)
            {
                EditorGUI.indentLevel++;
                DrawDebugData();
                EditorGUI.indentLevel--;
            }
        }

        private void DrawDebugData()
        {
            // State information
            _showStateData = EditorGUILayout.Foldout(_showStateData, "State Information", true);
            if (_showStateData && _controller.StateMachine != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Current State", _controller.StateMachine.CurrentStateType.ToString());
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Physics data
            _showPhysicsData = EditorGUILayout.Foldout(_showPhysicsData, "Physics Data", true);
            if (_showPhysicsData && _controller.Rb != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Vector3Field("Velocity", _controller.Rb.linearVelocity);
                EditorGUILayout.Vector3Field("Angular Velocity", _controller.Rb.angularVelocity);
                EditorGUILayout.FloatField("Speed", _controller.Rb.linearVelocity.magnitude);
                EditorGUILayout.FloatField("Horizontal Speed", new Vector3(_controller.Rb.linearVelocity.x, 0, _controller.Rb.linearVelocity.z).magnitude);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Movement data
            _showMovementData = EditorGUILayout.Foldout(_showMovementData, "Movement Data", true);
            if (_showMovementData && _controller.RbcMovement != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Toggle("Is Jumping", _controller.RbcMovement.IsJumping);
                EditorGUILayout.Toggle("Is Jump Force Active", _controller.RbcMovement.IsJumpForceActive);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Ground checker data
            _showGroundCheckerData = EditorGUILayout.Foldout(_showGroundCheckerData, "Ground Checker Data", true);
            if (_showGroundCheckerData && _controller.RbcGroundChecker != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Toggle("Is Grounded", _controller.RbcGroundChecker.IsGrounded);
                EditorGUILayout.FloatField("Distance To Ground", _controller.RbcGroundChecker.DistanceToGround);
                EditorGUILayout.FloatField("Sphere Cast Radius", _controller.RbcGroundChecker.SphereCastRadius);
                EditorGUILayout.FloatField("Max Ground Check Distance", _controller.RbcGroundChecker.MaxGroundCheckDistance);
                EditorGUILayout.Vector3Field("Last Ground Hit Point", _controller.RbcGroundChecker.LastGroundHitPoint);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Height spring data
            _showHeightSpringData = EditorGUILayout.Foldout(_showHeightSpringData, "Height Spring Data", true);
            if (_showHeightSpringData && _controller.RbcHeightSpring != null)
            {
                EditorGUI.indentLevel++;
                // Height spring doesn't expose many properties for debugging
                // We could add more properties to RbcHeightSpring if needed
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Camera data
            _showCameraData = EditorGUILayout.Foldout(_showCameraData, "Camera Data", true);
            if (_showCameraData && _controller.RbcCamera != null)
            {
                EditorGUI.indentLevel++;
                // Camera doesn't expose many properties for debugging
                // We could add more properties to RbcCamera if needed
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Draw a separator
            EditorGUILayout.Space(10);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space(10);

            // Input data
            EditorGUILayout.LabelField("Input Data", EditorStyles.boldLabel);
            if (_controller.InputHandler != null)
            {
                EditorGUILayout.Vector2Field("Move Input", _controller.InputHandler.MoveInput);
                EditorGUILayout.Vector2Field("Look Input", _controller.InputHandler.LookInput);
                EditorGUILayout.Toggle("Jump Pressed", _controller.InputHandler.JumpPressed);
                EditorGUILayout.Toggle("Jump Released", _controller.InputHandler.JumpButtonWasReleased);
                EditorGUILayout.Toggle("Shoot Pressed", _controller.InputHandler.ShootPressed);
            }
        }
    }
}
