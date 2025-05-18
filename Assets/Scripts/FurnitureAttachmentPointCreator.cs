using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRProject
{
    [RequireComponent(typeof(FurnitureAttachable))]
    public class FurnitureAttachmentPointCreator : MonoBehaviour
    {
        // Attachment point creation settings
        [Header("Generation Settings")]
        public bool createOnAwake = true;
        public bool clearExistingPoints = true;

        // Standard configurations
        [Header("Standard Configurations")]
        public bool isTableTop = false;
        public bool isTableLeg = false;
        public bool isChairSeat = false;
        public bool isChairBack = false;
        public bool isChairLeg = false;
        public bool isShelf = false;

        // Custom configuration
        [Header("Custom Configuration")]
        public bool useCustomConfig = false;
        public List<AttachmentPointConfig> customAttachmentPoints = new List<AttachmentPointConfig>();

        // Visual properties
        [Header("Visual Settings")]
        public Color gizmoColor = Color.yellow;
        public float gizmoSize = 0.02f;

        private FurnitureAttachable furnitureAttachable;

        [System.Serializable]
        public class AttachmentPointConfig
        {
            public string name = "AttachmentPoint";
            public Vector3 localPosition = Vector3.zero;
            public Vector3 localDirection = Vector3.up;
            public FurnitureAttachmentPoint.ConnectionType connectionType = FurnitureAttachmentPoint.ConnectionType.Universal;
            public string customType = "";
            public bool restrictConnectionTypes = false;
            public FurnitureAttachmentPoint.ConnectionType[] compatibleTypes;
        }

        private void Awake()
        {
            furnitureAttachable = GetComponent<FurnitureAttachable>();

            if (createOnAwake)
            {
                CreateAttachmentPoints();
            }
        }

        public void CreateAttachmentPoints()
        {
            if (furnitureAttachable == null)
            {
                furnitureAttachable = GetComponent<FurnitureAttachable>();
                if (furnitureAttachable == null)
                {
                    Debug.LogError("FurnitureAttachable component is required", this);
                    return;
                }
            }

            // Clear existing points if needed
            if (clearExistingPoints)
            {
                foreach (FurnitureAttachmentPoint point in furnitureAttachable.attachmentPoints)
                {
                    if (point != null && point.gameObject != null)
                    {
                        DestroyImmediate(point.gameObject);
                    }
                }
                furnitureAttachable.attachmentPoints.Clear();
            }

            // Generate based on the selected configuration
            if (useCustomConfig)
            {
                CreateCustomAttachmentPoints();
            }
            else
            {
                // Apply standard configurations
                if (isTableTop) CreateTableTopAttachmentPoints();
                if (isTableLeg) CreateTableLegAttachmentPoints();
                if (isChairSeat) CreateChairSeatAttachmentPoints();
                if (isChairBack) CreateChairBackAttachmentPoints();
                if (isChairLeg) CreateChairLegAttachmentPoints();
                if (isShelf) CreateShelfAttachmentPoints();
            }

            Debug.Log($"Created {furnitureAttachable.attachmentPoints.Count} attachment points on {gameObject.name}");
        }

        private void CreateCustomAttachmentPoints()
        {
            foreach (AttachmentPointConfig config in customAttachmentPoints)
            {
                CreateAttachmentPoint(
                    config.name,
                    config.localPosition,
                    config.localDirection,
                    config.connectionType,
                    config.customType,
                    config.restrictConnectionTypes,
                    config.compatibleTypes
                );
            }
        }

        private void CreateTableTopAttachmentPoints()
        {
            // Get object bounds to determine sizing
            Bounds bounds = CalculateBounds();
            float width = bounds.size.x;
            float length = bounds.size.z;
            float height = bounds.size.y;

            // Create attachment points at the four corners on the bottom face
            float cornerInset = width * 0.1f; // 10% inset from the corners

            // Bottom face connection points (for legs)
            CreateAttachmentPoint(
                "BottomLeft",
                new Vector3(-width/2 + cornerInset, -height/2, -length/2 + cornerInset),
                Vector3.forward, // Use forward direction for LookRotation
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down // This sets the up direction for the rotation
            );

            CreateAttachmentPoint(
                "BottomRight",
                new Vector3(width/2 - cornerInset, -height/2, -length/2 + cornerInset),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            CreateAttachmentPoint(
                "TopLeft",
                new Vector3(-width/2 + cornerInset, -height/2, length/2 - cornerInset),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            CreateAttachmentPoint(
                "TopRight",
                new Vector3(width/2 - cornerInset, -height/2, length/2 - cornerInset),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            // Top face connection point (for placing objects on top)
            CreateAttachmentPoint(
                "TableSurface",
                new Vector3(0, height/2, 0),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Top,
                "", false, null,
                Vector3.up
            );
        }

        private void CreateTableLegAttachmentPoints()
        {
            // Get object bounds to determine sizing
            Bounds bounds = CalculateBounds();
            float height = bounds.size.y;

            // Create a connection point at the top of the leg
            // For legs, we want the connection point's UP direction to be aligned with the leg's length
            // But LookRotation uses the forward (Z) direction, so we use a perpendicular vector
            // and rely on the up parameter to set the correct orientation
            CreateAttachmentPoint(
                "LegTop",
                new Vector3(0, height/2, 0),
                Vector3.forward, // Use forward direction for LookRotation
                FurnitureAttachmentPoint.ConnectionType.Leg,
                "", false, null,
                Vector3.up // This sets the up direction for the rotation
            );

            // Create a connection point at the bottom of the leg (to floor or other support)
            CreateAttachmentPoint(
                "LegBottom",
                new Vector3(0, -height/2, 0),
                Vector3.forward, // Use forward direction for LookRotation
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down // This sets the up direction for the rotation
            );
        }

        private void CreateChairSeatAttachmentPoints()
        {
            // Get object bounds to determine sizing
            Bounds bounds = CalculateBounds();
            float width = bounds.size.x;
            float length = bounds.size.z;
            float height = bounds.size.y;

            // Bottom face connection points (for legs)
            float cornerInset = width * 0.1f; // 10% inset from the corners

            CreateAttachmentPoint(
                "FrontLeftLeg",
                new Vector3(-width/2 + cornerInset, -height/2, -length/2 + cornerInset),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            CreateAttachmentPoint(
                "FrontRightLeg",
                new Vector3(width/2 - cornerInset, -height/2, -length/2 + cornerInset),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            CreateAttachmentPoint(
                "BackLeftLeg",
                new Vector3(-width/2 + cornerInset, -height/2, length/2 - cornerInset),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            CreateAttachmentPoint(
                "BackRightLeg",
                new Vector3(width/2 - cornerInset, -height/2, length/2 - cornerInset),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            // Top face connection point (for sitting)
            CreateAttachmentPoint(
                "SeatSurface",
                new Vector3(0, height/2, 0),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Top,
                "", false, null,
                Vector3.up
            );

            // Back connection point (for chair back)
            CreateAttachmentPoint(
                "BackConnection",
                new Vector3(0, height/2, length/2),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Back,
                "", false, null,
                Vector3.up
            );
        }

        private void CreateChairBackAttachmentPoints()
        {
            // Get object bounds to determine sizing
            Bounds bounds = CalculateBounds();
            float width = bounds.size.x;
            float height = bounds.size.y;

            // Connection point to the seat
            CreateAttachmentPoint(
                "SeatConnection",
                new Vector3(0, -height/2, 0),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Back,
                "", false, null,
                Vector3.down
            );
        }

        private void CreateChairLegAttachmentPoints()
        {
            // Similar to table leg
            CreateTableLegAttachmentPoints();
        }

        private void CreateShelfAttachmentPoints()
        {
            // Get object bounds to determine sizing
            Bounds bounds = CalculateBounds();
            float width = bounds.size.x;
            float height = bounds.size.y;
            float length = bounds.size.z;

            // Top surface
            CreateAttachmentPoint(
                "TopSurface",
                new Vector3(0, height/2, 0),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Top,
                "", false, null,
                Vector3.up
            );

            // Bottom surface
            CreateAttachmentPoint(
                "BottomSurface",
                new Vector3(0, -height/2, 0),
                Vector3.forward,
                FurnitureAttachmentPoint.ConnectionType.Bottom,
                "", false, null,
                Vector3.down
            );

            // Side connections for wall mount or connecting to other shelves
            CreateAttachmentPoint(
                "LeftSide",
                new Vector3(-width/2, 0, 0),
                Vector3.up,
                FurnitureAttachmentPoint.ConnectionType.Side,
                "", false, null,
                Vector3.left
            );

            CreateAttachmentPoint(
                "RightSide",
                new Vector3(width/2, 0, 0),
                Vector3.up,
                FurnitureAttachmentPoint.ConnectionType.Side,
                "", false, null,
                Vector3.right
            );

            CreateAttachmentPoint(
                "BackSide",
                new Vector3(0, 0, -length/2),
                Vector3.up,
                FurnitureAttachmentPoint.ConnectionType.Side,
                "", false, null,
                Vector3.back
            );
        }

        private FurnitureAttachmentPoint CreateAttachmentPoint(
            string name,
            Vector3 localPosition,
            Vector3 direction,
            FurnitureAttachmentPoint.ConnectionType connectionType,
            string customType = "",
            bool restrictConnectionTypes = false,
            FurnitureAttachmentPoint.ConnectionType[] compatibleTypes = null,
            Vector3? upDirection = null)
        {
            // Create a new GameObject for the attachment point
            GameObject pointObject = new GameObject(name);
            pointObject.transform.parent = transform;
            pointObject.transform.localPosition = localPosition;

            // Set the forward direction
            if (upDirection.HasValue)
            {
                // If an up direction is provided, use it for more precise orientation
                pointObject.transform.localRotation = Quaternion.LookRotation(direction, upDirection.Value);
            }
            else
            {
                pointObject.transform.localRotation = Quaternion.LookRotation(direction);
            }

            // Add the attachment point component
            FurnitureAttachmentPoint attachPoint = pointObject.AddComponent<FurnitureAttachmentPoint>();
            attachPoint.parentFurniture = furnitureAttachable;
            attachPoint.connectionType = connectionType;
            attachPoint.customType = customType;
            attachPoint.restrictConnectionTypes = restrictConnectionTypes;
            attachPoint.compatibleTypes = compatibleTypes;
            attachPoint.indicatorColor = gizmoColor;
            attachPoint.indicatorSize = gizmoSize;

            // Add to the furniture's list of attachment points
            furnitureAttachable.attachmentPoints.Add(attachPoint);

            return attachPoint;
        }

        // Helper method to calculate object bounds
        private Bounds CalculateBounds()
        {
            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                // Start with the first renderer's bounds
                bounds = renderers[0].bounds;

                // Expand to include all other renderers
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }
            else
            {
                // Fallback if no renderers - use colliders
                Collider[] colliders = GetComponentsInChildren<Collider>();
                if (colliders.Length > 0)
                {
                    bounds = colliders[0].bounds;
                    for (int i = 1; i < colliders.Length; i++)
                    {
                        bounds.Encapsulate(colliders[i].bounds);
                    }
                }
                else
                {
                    // No renderers or colliders, use transform as center with a default size
                    bounds.center = transform.position;
                    bounds.size = new Vector3(1, 1, 1);
                    Debug.LogWarning("No Renderers or Colliders found to calculate bounds. Using default size.", this);
                }
            }

            // Convert to local space bounds
            Vector3 localCenter = transform.InverseTransformPoint(bounds.center);
            Vector3 localSize = bounds.size;

            // Handle scale
            localSize.x /= transform.lossyScale.x;
            localSize.y /= transform.lossyScale.y;
            localSize.z /= transform.lossyScale.z;

            return new Bounds(localCenter, localSize);
        }

        #if UNITY_EDITOR
        // Draw gizmos for debugging
        private void OnDrawGizmosSelected()
        {
            if (furnitureAttachable == null)
                furnitureAttachable = GetComponent<FurnitureAttachable>();

            Bounds bounds = CalculateBounds();

            // Draw a wireframe to show the calculated bounds
            Gizmos.color = Color.cyan;
            Vector3 worldCenter = transform.TransformPoint(bounds.center);
            Gizmos.DrawWireCube(worldCenter, bounds.size);
        }
        #endif
    }

    #if UNITY_EDITOR
    // Custom editor for easier attachment point creation
    [UnityEditor.CustomEditor(typeof(FurnitureAttachmentPointCreator))]
    public class FurnitureAttachmentPointCreatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            FurnitureAttachmentPointCreator creator = (FurnitureAttachmentPointCreator)target;

            UnityEditor.EditorGUILayout.Space();
            if (GUILayout.Button("Create Attachment Points"))
            {
                creator.CreateAttachmentPoints();
            }
        }
    }
    #endif
}