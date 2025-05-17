# Poke Interactors for XR Interaction Toolkit

This package adds poke interactors to the left and right controllers in an XR project, similar to how the ray interactors work. Poke interactors allow for direct touch-based interaction with UI elements and interactable objects.

## Setup Instructions

### Automatic Setup (Recommended)

1. Add the `PokeInteractorManager` component to any GameObject in your scene
2. In the Inspector, you can either:
   - Manually assign the left and right controller anchors, or
   - Click the "Find Controller Anchors" button to auto-detect them
3. Click "Setup Poke Interactors" to create and configure the poke interactors
4. Adjust the poke interactor settings as needed
5. Click "Update Poke Configuration" to apply your changes
6. Use "Toggle Visual Indicators" to show/hide visual indicators for debugging

### Manual Setup

If you prefer to set up poke interactors manually:

1. Add the `SetupPokeInteractors` component to any GameObject in the scene
2. Assign the `leftHandAnchor` and `rightHandAnchor` references
3. Click the "Setup Interactors" button in the Inspector
4. To configure the poke interactors:   - Add the `ConfigurePokeInteractors` component to a GameObject
   - Adjust the settings as desired
   - Click the "Update Poke Interactors" button

## Integration with Interaction Groups

The poke interactors are automatically integrated with your scene's interaction groups. The priority order is:
1. Direct interactors (highest priority)
2. Poke interactors (medium priority)
3. Ray interactors (lowest priority)

This means that direct manipulation takes precedence over poke interaction, which takes precedence over ray interaction.

## Settings

The poke interactors can be configured with the following settings:

- **Poke Depth**: How far the poke extends from the controller
- **Poke Width**: The width of the poke interactor
- **Poke Select Width**: The width used for selection
- **Poke Hover Radius**: The radius used for hover detection
- **Poke Interaction Offset**: The offset used for interaction
- **Enable UI Interaction**: Whether to allow interaction with UI elements
- **Debug Visualizations**: Whether to show debug visualizations
- **Show Visual Indicator**: Whether to show the visual indicator sphere

## Troubleshooting

If you experience issues:

1. **Poke interactors not being created**: Make sure the controller anchors are assigned correctly
2. **Poke interactors not interacting**: Check that they're added to the correct interaction groups
3. **Priority issues**: Use the `PokeInteractorManager` to auto-order interactors by priority
4. **Visibility issues**: Toggle the visual indicators to see the poke points

For further assistance, refer to the XR Interaction Toolkit documentation.
