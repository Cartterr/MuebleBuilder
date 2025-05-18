using UnityEngine;

namespace VRProject
{
    public class FurnitureAttachmentPoint : MonoBehaviour
    {
        public FurnitureAttachable parentFurniture;
        public FurnitureAttachmentPoint connectedTo;

        // Type of connection this point supports
        public enum ConnectionType
        {
            Universal,   // Can connect to any type
            Leg,         // Leg connection point (table leg, chair leg)
            Top,         // Top connection point (table top, chair seat)
            Back,        // Back connection point (chair back)
            Side,        // Side connection point (shelf side, cabinet side)
            Bottom,      // Bottom connection point
            Custom       // Custom type (use customType string)
        }

        public ConnectionType connectionType = ConnectionType.Universal;
        public string customType = ""; // Used if connectionType is Custom

        // Connection state
        public bool isConnected = false;

        // Visual indicator for debugging/editor
        public bool showVisualIndicator = true;
        public Color indicatorColor = Color.yellow;
        public float indicatorSize = 0.02f;

        // Connection compatibility settings
        public bool restrictConnectionTypes = false;
        public ConnectionType[] compatibleTypes; // Types this point can connect to

        private void OnDrawGizmos()
        {
            if (showVisualIndicator)
            {
                // Draw a sphere at the attachment point
                Gizmos.color = isConnected ? Color.green : indicatorColor;
                Gizmos.DrawSphere(transform.position, indicatorSize);

                // Draw a line showing the forward direction
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, transform.forward * indicatorSize * 2);

                // Draw a line showing the up direction
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, transform.up * indicatorSize * 2);

                // Draw a line showing the right direction
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.right * indicatorSize * 2);

                // Draw connection type label
                #if UNITY_EDITOR
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(transform.position, GetConnectionTypeName());
                #endif
            }
        }

        // Get a readable name for the connection type
        private string GetConnectionTypeName()
        {
            if (connectionType == ConnectionType.Custom)
                return customType;
            return connectionType.ToString();
        }

        // Check if this point can connect to another point
        public bool CanConnectTo(FurnitureAttachmentPoint other)
        {
            // Basic checks
            if (other == null || other == this || isConnected || other.isConnected)
                return false;

            // If either is universal and not restricted, they can connect
            if (!restrictConnectionTypes && connectionType == ConnectionType.Universal)
                return true;

            if (!other.restrictConnectionTypes && other.connectionType == ConnectionType.Universal)
                return true;

            // Specific compatibility rules (if restrictions are enabled)
            if (restrictConnectionTypes)
            {
                // Check if the other's type is in our list of compatible types
                if (compatibleTypes != null)
                {
                    foreach (ConnectionType type in compatibleTypes)
                    {
                        if (other.connectionType == type)
                            return true;

                        // Handle custom types
                        if (type == ConnectionType.Custom && other.connectionType == ConnectionType.Custom)
                            return customType == other.customType;
                    }
                }
                return false; // Not in our compatibility list
            }

            // Default compatibility rules - add your furniture-specific rules here
            // For example: legs connect to bottoms, tops connect to sides, etc.
            switch (connectionType)
            {
                case ConnectionType.Leg:
                    return other.connectionType == ConnectionType.Bottom;

                case ConnectionType.Top:
                    return other.connectionType == ConnectionType.Leg ||
                           other.connectionType == ConnectionType.Side;

                case ConnectionType.Back:
                    return other.connectionType == ConnectionType.Side ||
                           other.connectionType == ConnectionType.Top;

                case ConnectionType.Side:
                    return other.connectionType == ConnectionType.Side ||
                           other.connectionType == ConnectionType.Top ||
                           other.connectionType == ConnectionType.Bottom;

                case ConnectionType.Bottom:
                    return other.connectionType == ConnectionType.Leg ||
                           other.connectionType == ConnectionType.Side;

                case ConnectionType.Custom:
                    // Custom types must match
                    return other.connectionType == ConnectionType.Custom &&
                           customType == other.customType;

                default:
                    return true; // Universal can connect to anything
            }
        }

        // Helper method to create a connection with another point
        public void Connect(FurnitureAttachmentPoint other)
        {
            if (!CanConnectTo(other))
                return;

            connectedTo = other;
            isConnected = true;

            other.connectedTo = this;
            other.isConnected = true;
        }

        // Disconnect from the currently connected point
        public void Disconnect()
        {
            if (connectedTo != null)
            {
                connectedTo.isConnected = false;
                connectedTo.connectedTo = null;

                isConnected = false;
                connectedTo = null;
            }
        }

        private void OnDestroy()
        {
            // Make sure we disconnect when destroyed
            Disconnect();
        }
    }
}