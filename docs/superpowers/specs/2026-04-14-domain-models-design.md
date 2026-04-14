# Design Spec: Domain Models — 2D Top-Down View

**Date:** 2026-04-14
**Topic:** OOP Foundation — Core domain model with full class hierarchy

## Overview

Implement the core domain models for the Sims Constructor application using a 2D top-down view. All objects are viewed from above, positioned on a 2D plane using (X, Y) coordinates. The design demonstrates all four OOP pillars: Encapsulation, Inheritance, Polymorphism, and Abstraction.

## Folder Structure

```
Models/
├── Core/
│   └── GameObject.cs          # Abstract base class for all game objects
├── Items/
│   ├── RoomItem.cs            # Abstract: adds position, rotation, dimensions
│   ├── FurnitureItem.cs       # Bed, table, chair, sofa, wardrobe, etc.
│   ├── ApplianceItem.cs       # TV, fridge, washer, microwave, etc.
│   └── DecorationItem.cs      # Rug, plant, painting, lamp, etc.
└── Interfaces/
    ├── IPlaceable.cs           # Can be placed at a position
    ├── IRotatable.cs           # Can be rotated
    ├── IPricable.cs            # Has a cost
    └── ISelectable.cs          # Can be selected/deselected
```

## Class Hierarchy

### GameObject (abstract base)

**Properties:**
- `Guid Id` — unique identifier (read-only after creation)
- `string Name` — display name
- `string Description` — optional description
- `Color Color` — visual color

**Methods:**
- `abstract Rectangle GetRenderBounds()` — returns bounding rectangle for rendering
- `override string ToString()` — returns formatted name

**OOP — Abstraction:** Abstract class, cannot be instantiated directly.
**OOP — Polymorphism:** `GetRenderBounds()` overridden in derived classes.

---

### RoomItem (abstract, inherits GameObject)

**Properties:**
- `float X` — horizontal position (validated, min 0)
- `float Y` — vertical position (validated, min 0)
- `float Width` — item width in room units
- `float Height` — item height in room units
- `int Rotation` — 0, 90, 180, or 270 degrees
- `bool IsSelected` — selection state (private set)
- `bool IsPlaced` — whether item has been placed in the room

**Methods:**
- `void PlaceAt(float x, float y)` — validates and sets position (encapsulation)
- `void Rotate90()` — rotates clockwise by 90°, swaps Width/Height
- `void Select()` — sets IsSelected = true
- `void Deselect()` — sets IsSelected = false
- `override Rectangle GetRenderBounds()` — returns (X, Y, Width, Height)

**OOP — Encapsulation:** Position validated in `PlaceAt()`. `IsSelected` has private setter. `IsPlaced` computed from valid coordinates.
**OOP — Inheritance:** Extends `GameObject` with spatial properties.

---

### FurnitureItem (inherits RoomItem)

**Properties:**
- `FurnitureCategory Category` — enum: Bed, Seating, Storage, Surface, Table
- `string Material` — wood, metal, plastic, glass, etc.
- `PlacementRule PlacementRule` — WallRequired, FloorOnly, CornerPreferred

**Methods:**
- `bool CanPlaceNear(RoomItem other)` — checks if this furniture can be near another
- `override string ToString()` — returns "Furniture: {Name} ({Category})"

---

### ApplianceItem (inherits RoomItem)

**Properties:**
- `decimal PowerConsumption` — watts, 0 for non-electrical
- `bool RequiresWater` — e.g., washing machine, dishwasher
- `bool RequiresVentilation` — e.g., oven, dryer

**Methods:**
- `override string ToString()` — returns "Appliance: {Name}"

---

### DecorationItem (inherits RoomItem)

**Properties:**
- `DecorationStyle Style` — enum: Modern, Classic, Rustic, Minimalist, Eclectic
- `bool IsWallMountable` — can be mounted on walls (paintings, shelves)

**Methods:**
- `override string ToString()` — returns "Decoration: {Name} ({Style})"

---

## Interfaces

### IPlaceable
```csharp
public interface IPlaceable
{
    bool CanPlaceAt(float x, float y);
    void PlaceAt(float x, float y);
    bool IsPlaced { get; }
}
```

### IRotatable
```csharp
public interface IRotatable
{
    void Rotate();
    int Rotation { get; }
    int GetRotationDegrees();
}
```

### IPricable
```csharp
public interface IPricable
{
    decimal GetPrice();
}
```

### ISelectable
```csharp
public interface ISelectable
{
    void Select();
    void Deselect();
    bool IsSelected { get; }
}
```

## 2D Top-Down Coordinate System

- **View:** Top-down orthographic projection
- **Origin:** Top-left corner of the room (0, 0)
- **X axis:** Increases left to right
- **Y axis:** Increases top to bottom
- **Units:** Meters (logical), converted to pixels for rendering
- **Room dimensions:** Configurable, e.g., 5m × 4m
- **Rotation:** 0° = default, 90° = clockwise swap of Width/Height

## OOP Pillars Summary

| Pillar | Implementation |
|--------|---------------|
| **Encapsulation** | Private setters on `IsSelected`, `Id`; validation in `PlaceAt()`; protected internal state |
| **Inheritance** | `GameObject` → `RoomItem` → `FurnitureItem` / `ApplianceItem` / `DecorationItem` |
| **Polymorphism** | Overridden `GetRenderBounds()`, `ToString()`; shared interface contracts |
| **Abstraction** | 2 abstract classes; 4 interfaces defining behavior contracts |

## Enums

```csharp
public enum FurnitureCategory
{
    Bed,
    Seating,
    Storage,
    Surface,
    Table
}

public enum PlacementRule
{
    WallRequired,      // Must be placed against a wall (bed, wardrobe)
    FloorOnly,         // Can be placed anywhere (table, chair)
    CornerPreferred    // Prefer corners (plant, lamp)
}

public enum DecorationStyle
{
    Modern,
    Classic,
    Rustic,
    Minimalist,
    Eclectic
}
```

## Self-Review

- **Placeholders:** None. All classes, properties, methods, and enums defined.
- **Consistency:** Class hierarchy is consistent. Interfaces align with class capabilities.
- **Scope:** Focused on domain models only — no UI, no services. Appropriate for a single implementation plan.
- **Ambiguity:** Units specified (meters for logical, pixels for rendering). Rotation direction specified (clockwise).
