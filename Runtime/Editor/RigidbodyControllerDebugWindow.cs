using System.Collections.Generic;

using JL_GameProdEnv_CustomPackage.Runtime.RBController;

using UnityEditor;

using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.Editor
{
    public class RigidbodyControllerDebugWindow : EditorWindow
    {
        private RigidbodyController _controller;
        private Vector2 _scrollPosition;
        private bool _showPhysicsData = true;
        private bool _showMovementData = true;
        private bool _showGroundCheckerData = true;
        private bool _showInputData = true;

        // History tracking
        private const int MAX_HISTORY_LENGTH = 100;
        private Dictionary<string, List<bool>> _boolHistory = new();

        [MenuItem("Tools/RBController/Debug Window")]
        public static void ShowWindow()
        {
            GetWindow<RigidbodyControllerDebugWindow>("RBC Debug");
        }

        private void OnEnable()
        {
            EditorApplication.update += UpdateBooleanHistory;
            titleContent = new GUIContent("RBC Debug");
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateBooleanHistory;
        }

        private void UpdateBooleanHistory()
        {
            if (!Application.isPlaying || _controller == null) return;

            UpdateBoolValue("Is Grounded", _controller.RbcGroundChecker?.IsGrounded ?? false);
            UpdateBoolValue("Is Jumping", _controller.RbcMovement?.IsJumping ?? false);
            UpdateBoolValue("Is Jump Force Active", _controller.RbcMovement?.IsJumpForceActive ?? false);
            UpdateBoolValue("Jump Pressed", _controller.InputHandler?.JumpPressed ?? false);
            UpdateBoolValue("Jump Released", _controller.InputHandler?.JumpButtonWasReleased ?? false);
            UpdateBoolValue("Jump Button Was Released", _controller.InputHandler?.JumpButtonWasReleased ?? false);
            UpdateBoolValue("Shoot Pressed", _controller.InputHandler?.ShootPressed ?? false);
        }

        private void UpdateBoolValue(string key, bool value)
        {
            if (!_boolHistory.ContainsKey(key))
            {
                _boolHistory[key] = new List<bool>();
            }

            var history = _boolHistory[key];
            history.Add(value);
            
            if (history.Count > MAX_HISTORY_LENGTH)
            {
                history.RemoveAt(0);
            }
        }

        private void OnGUI()
        {
            // Find RigidbodyController in scene if not already assigned
            if (_controller == null)
            {
                _controller = FindFirstObjectByType<RigidbodyController>();
            }

            if (_controller == null)
            {
                EditorGUILayout.HelpBox("No RigidbodyController found in scene.", MessageType.Warning);
                return;
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter play mode to see debug data.", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Physics Data
            _showPhysicsData = EditorGUILayout.Foldout(_showPhysicsData, "Physics Data", true);
            if (_showPhysicsData && _controller.Rb != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Vector3Field("Velocity", _controller.Rb.linearVelocity);
                EditorGUILayout.Vector3Field("Angular Velocity", _controller.Rb.angularVelocity);
                EditorGUILayout.FloatField("Speed", _controller.Rb.linearVelocity.magnitude);
                EditorGUILayout.FloatField("Horizontal Speed", 
                    new Vector3(_controller.Rb.linearVelocity.x, 0, _controller.Rb.linearVelocity.z).magnitude);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Movement Data
            _showMovementData = EditorGUILayout.Foldout(_showMovementData, "Movement Data", true);
            if (_showMovementData && _controller.RbcMovement != null)
            {
                EditorGUI.indentLevel++;
                DrawBooleanField("Is Jumping", _controller.RbcMovement.IsJumping, Color.green);
                DrawBooleanField("Is Jump Force Active", _controller.RbcMovement.IsJumpForceActive, Color.cyan);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Ground Checker Data
            _showGroundCheckerData = EditorGUILayout.Foldout(_showGroundCheckerData, "Ground Checker Data", true);
            if (_showGroundCheckerData && _controller.RbcGroundChecker != null)
            {
                EditorGUI.indentLevel++;
                DrawBooleanField("Is Grounded", _controller.RbcGroundChecker.IsGrounded, Color.yellow);
                EditorGUILayout.FloatField("Distance To Ground", _controller.RbcGroundChecker.DistanceToGround);
                EditorGUILayout.FloatField("Sphere Cast Radius", _controller.RbcGroundChecker.SphereCastRadius);
                EditorGUILayout.FloatField("Max Ground Check Distance", _controller.RbcGroundChecker.MaxGroundCheckDistance);
                EditorGUILayout.Vector3Field("Last Ground Hit Point", _controller.RbcGroundChecker.LastGroundHitPoint);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            // Input Data
            _showInputData = EditorGUILayout.Foldout(_showInputData, "Input Data", true);
            if (_showInputData && _controller.InputHandler != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Vector2Field("Move Input", _controller.InputHandler.MoveInput);
                EditorGUILayout.Vector2Field("Look Input", _controller.InputHandler.LookInput);
                DrawBooleanField("Jump Pressed", _controller.InputHandler.JumpPressed, Color.magenta);
                DrawBooleanField("Jump Released", _controller.InputHandler.JumpButtonWasReleased, Color.red);
                DrawBooleanField("Jump Button Was Released", _controller.InputHandler.JumpButtonWasReleased, Color.blue);
                DrawBooleanField("Shoot Pressed", _controller.InputHandler.ShootPressed, Color.white);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();

            // Force repaint to update graphs
            Repaint();
        }

        private void DrawBooleanField(string label, bool value, Color graphColor)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Toggle(label, value);
            EditorGUILayout.EndHorizontal();

            if (_boolHistory.ContainsKey(label))
            {
                DrawBooleanGraph(label + " History", _boolHistory[label], graphColor);
            }
        }

        private void DrawBooleanGraph(string label, List<bool> history, Color color)
        {
            EditorGUILayout.LabelField(label);

            Rect graphRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40, 50);

            if (Event.current.type == EventType.Repaint)
            {
                // Draw background
                EditorGUI.DrawRect(graphRect, new Color(0.1f, 0.1f, 0.1f, 1));

                // Draw grid line for on/off
                Rect midLineRect = new Rect(graphRect.x, graphRect.y + graphRect.height * 0.5f, graphRect.width, 1);
                EditorGUI.DrawRect(midLineRect, new Color(0.3f, 0.3f, 0.3f, 1));

                if (history != null && history.Count > 0)
                {
                    // Draw graph
                    float pointWidth = graphRect.width / history.Count;

                    for (int i = 0; i < history.Count; i++)
                    {
                        float height = history[i] ? graphRect.height * 0.8f : 0;
                        float yPos = history[i] ? graphRect.y + graphRect.height * 0.1f : 
                            graphRect.y + graphRect.height * 0.9f;

                        Rect pointRect = new Rect(
                            graphRect.x + i * pointWidth, 
                            yPos, 
                            Mathf.Max(1, pointWidth - 1), 
                            height);

                        EditorGUI.DrawRect(pointRect, color);
                    }
                }
            }
        }
    }
}