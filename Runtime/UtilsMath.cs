using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime
{
    public static class UtilsMath
    {
        /// <summary>
        /// Finds the shortest rotation from one rotation to another.
        /// </summary>
        /// <param name="from">Starting rotation</param>
        /// <param name="to">Target rotation</param>
        /// <returns>The shortest rotation that rotates from the start to target rotation</returns>
        public static Quaternion ShortestRotation(Quaternion from, Quaternion to)
        {
            // Calculate the rotation needed to go from the "from" rotation to the "to" rotation
            Quaternion resultRotation = Quaternion.Inverse(from) * to;
            
            // Ensure we're taking the shortest path
            // If the dot product is negative, we need to negate the rotation
            if (Quaternion.Dot(Quaternion.identity, resultRotation) < 0)
            {
                resultRotation.x = -resultRotation.x;
                resultRotation.y = -resultRotation.y;
                resultRotation.z = -resultRotation.z;
                resultRotation.w = -resultRotation.w;
            }
            
            return resultRotation;
        }
        
        // You can add more math utility methods here as needed
        // For example:
        
        /// <summary>
        /// Remaps a value from one range to another.
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            // Convert the value from the first range to a percentage
            float percentage = Mathf.InverseLerp(fromMin, fromMax, value);
            
            // Convert the percentage to the second range
            return Mathf.Lerp(toMin, toMax, percentage);
        }
        
        /// <summary>
        /// Calculates a smooth damping between two rotations.
        /// </summary>
        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, 
                                                     ref Vector3 angularVelocity, float smoothTime)
        {
            float dt = Time.deltaTime;
            
            // Get the difference between rotations
            Quaternion diffRotation = target * Quaternion.Inverse(current);
            diffRotation.ToAngleAxis(out float angle, out Vector3 axis);
            
            // Handle angle overflow
            if (angle > 180f)
                angle -= 360f;
            
            // Calculate the target angular velocity
            Vector3 targetAngularVelocity = angle * axis * Mathf.Deg2Rad / smoothTime;
            
            // Dampen current velocity toward target
            angularVelocity = Vector3.Lerp(angularVelocity, targetAngularVelocity, dt / smoothTime);
            
            // Apply velocity
            float angularChange = angularVelocity.magnitude * dt;
            Vector3 rotationAxis = angularVelocity.normalized;
            
            Quaternion deltaRotation = Quaternion.AngleAxis(angularChange * Mathf.Rad2Deg, rotationAxis);
            
            return deltaRotation * current;
        }
    }
}
