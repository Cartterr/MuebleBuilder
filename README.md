# 🧩 LEGO-Style Attachment System for Meta Quest 3 MR Template 🧩

## ⚠️ FORK NOTICE ⚠️
**This project is a fork of [CorePolygon's unity-mr-template-quest3](https://github.com/corepolygon/unity-mr-template-quest3)**
Original work by [@kpansavuth201](https://github.com/kpansavuth201) and [@corepolygon](https://github.com/corepolygon)

![License](https://img.shields.io/badge/license-GPL--3.0-blue)
![Unity Version](https://img.shields.io/badge/Unity-2022.3.20f1-lightgrey)
![Platform](https://img.shields.io/badge/Platform-Quest%202%2F3-purple)

---

## 🔍 Overview

This fork extends the original Mixed-Reality Template for Meta Quest 3 with a **custom attachment system** that allows VR objects to connect like building blocks. The system enables users to create complex structures in mixed reality by snapping objects together and manipulating them as unified structures.

![Attachment System Demo](https://via.placeholder.com/600x300?text=Attachment+System+Demo)

### 🧊 Key Features Added:

- **LEGO-style Attachment System**: Grab and connect objects in VR
- **Shared Anchor Mechanism**: Connected objects behave as a single entity
- **Easy Setup**: Simple component-based integration
- **Interactive MR Building**: Create structures in your physical space

---

## 🏗️ Project Details

### 📋 Original Project Information
| | |
|---|---|
| **Client** | Core Polygon |
| **Project Name** | Mixed-Reality Template for Meta Quest 3 |
| **Description** | Unity Mixed-Reality Template for Meta Quest 3 |
| **Dev Platform** | Unity 2022.3.20f1 |
| **Build Platform** | Android |
| **Build Scene** | Assets/CorePolygonMRTemplate/Scenes/demo |
| **Hardware** | Meta Quest 2, Meta Quest 3 |

### 🧰 Technical Stack

| | |
|---|---|
| **Third Party Framework/Plugins** | XR Interaction Toolkit, XR Hand, AR Foundation |
| **Package Manager** | None |

---

## 🔧 Custom Components

This fork adds the following custom components:

### 📦 AttachableCube Component
Turns any object into an attachable block that can be grabbed in VR and connected to other attachable objects.

### 📍 AttachmentPoint Component
Creates connection points on attachable objects, defining where and how objects can be connected.

### 🔄 Shared Anchor System
When objects connect, they form a shared anchor that allows the entire structure to be manipulated as one unit.

---

## 🚀 Getting Started

### 📋 Prerequisites
- Unity 2022.3.20f1 or newer
- Mixed Reality Template Quick Start Guide: [Unity Documentation](https://docs.unity3d.com/Packages/com.unity.template.mixed-reality@1.0/manual/index.html)
- For AR Plane detection, complete Physical Space Setup in Meta Quest 3

### 🛠️ Setup Instructions

1. Clone this repository
2. Open in Unity 2022.3.20f1+
3. Add the "AttachmentPoint" tag in your Unity project
4. To create attachable objects:
   - Add `AttachableCube` script to any object with a Rigidbody
   - Add `AttachmentPointCreator` to generate attachment points
   - Optionally add `AttachmentPointVisualizer` for visual feedback

### 📝 Example Usage
```csharp
// 1. Create a cube with attachment capability
public GameObject cubePrefab;

void CreateAttachableCube()
{
    GameObject cube = Instantiate(cubePrefab);

    // Add required components
    AttachableCube attachable = cube.AddComponent<AttachableCube>();

    // Generate attachment points
    AttachmentPointCreator creator = cube.AddComponent<AttachmentPointCreator>();
    creator.CreateAttachmentPoints();
}
```

---

## 📝 Additional Notes

### 🔣 Scripting Symbols
- DEBUG_ENABLED : Example debug

### 🎮 Cheat Codes
- CHEAT_EXAMPLE_SYMBOL : Example description

---

## ⭐ Credits & Acknowledgments

### 🎖️ Original Project Team
- [CorePolygon](https://github.com/corepolygon) - Original template creation
- [kpansavuth201](https://github.com/kpansavuth201) - Initial project commit

### 🔧 Fork Contributors
- [Your Name/Username] - Implementation of the attachment system

### 📚 Resources & Inspiration
- [Unity XR Interaction Toolkit Documentation](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/index.html)
- [Meta Quest Developer Hub](https://developer.oculus.com/quest/)

---

## 📄 License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

The original work is copyright © 2023 Core Polygon, licensed under GPL-3.0.

---

## 📞 Contact

For questions about this fork, please [open an issue](https://github.com/yourusername/unity-mr-template-quest3/issues) on GitHub.

For inquiries about the original template, contact [Core Polygon](https://github.com/corepolygon).

---

<p align="center">
<img src="https://via.placeholder.com/100x100?text=Your+Logo" alt="Your Logo" width="80"/>
</p>

<p align="center">
Built with ❤️ for the XR community
</p>
