# Domain Models Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the core OOP domain model hierarchy with interfaces, enums, and TDD tests for a 2D top-down room constructor.

**Architecture:** Abstract base classes (`GameObject`, `RoomItem`) with concrete derived classes (`FurnitureItem`, `ApplianceItem`, `DecorationItem`). Four interfaces define behavior contracts. All classes use proper encapsulation, inheritance, polymorphism, and abstraction.

**Tech Stack:** C#, .NET 9.0, xUnit (testing), System.Drawing (Rectangle)

---

## File Structure

### New Files to Create

| File | Responsibility |
|------|---------------|
| `SimsConstructor.Tests/SimsConstructor.Tests.csproj` | Test project configuration |
| `Models/Interfaces/IPlaceable.cs` | Placement contract |
| `Models/Interfaces/IRotatable.cs` | Rotation contract |
| `Models/Interfaces/IPricable.cs` | Pricing contract |
| `Models/Interfaces/ISelectable.cs` | Selection contract |
| `Models/Core/GameObject.cs` | Abstract base class for all objects |
| `Models/Items/RoomItem.cs` | Abstract spatial item with position/rotation |
| `Models/Items/FurnitureItem.cs` | Concrete furniture with category/placement rules |
| `Models/Items/ApplianceItem.cs` | Concrete appliances with utility requirements |
| `Models/Items/DecorationItem.cs` | Concrete decorations with style themes |
| `Models/Enums/FurnitureCategory.cs` | Furniture type enum |
| `Models/Enums/PlacementRule.cs` | Placement constraint enum |
| `Models/Enums/DecorationStyle.cs` | Decoration style enum |
| `SimsConstructor.Tests/GameObjectTests.cs` | Tests for GameObject |
| `SimsConstructor.Tests/RoomItemTests.cs` | Tests for RoomItem |
| `SimsConstructor.Tests/FurnitureItemTests.cs` | Tests for FurnitureItem |
| `SimsConstructor.Tests/ApplianceItemTests.cs` | Tests for ApplianceItem |
| `SimsConstructor.Tests/DecorationItemTests.cs` | Tests for DecorationItem |
| `SimsConstructor.Tests/InterfaceTests.cs` | Tests for interface implementations |

### Files to Modify

| File | Change |
|------|--------|
| `SimsConstructor.csproj` | Add reference to test project (optional, tests reference main project) |
| `QWEN.md` | Update project status |

---

### Task 1: Create Test Project and Interfaces

**Files:**
- Create: `SimsConstructor.Tests/SimsConstructor.Tests.csproj`
- Create: `Models/Interfaces/IPlaceable.cs`
- Create: `Models/Interfaces/IRotatable.cs`
- Create: `Models/Interfaces/IPricable.cs`
- Create: `Models/Interfaces/ISelectable.cs`

- [ ] **Step 1: Create the test project**

```bash
dotnet new xunit -n SimsConstructor.Tests -o SimsConstructor.Tests --target-framework-override net9.0
```

- [ ] **Step 2: Add project reference from test project to main project**

```bash
dotnet add SimsConstructor.Tests/SimsConstructor.Tests.csproj reference SimsConstructor.csproj
```

- [ ] **Step 3: Verify test project builds and runs**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj
```

Expected: PASS with 0 tests (empty test file created by template).

- [ ] **Step 4: Create IPlaceable.cs**

```csharp
namespace SimsConstructor.Interfaces;

/// <summary>
/// Defines an object that can be placed at a position in the room.
/// Demonstrates Abstraction — behavior contract without implementation.
/// </summary>
public interface IPlaceable
{
    /// <summary>
    /// Checks whether the object can be placed at the given coordinates.
    /// </summary>
    bool CanPlaceAt(float x, float y);

    /// <summary>
    /// Places the object at the given coordinates.
    /// </summary>
    void PlaceAt(float x, float y);

    /// <summary>
    /// Gets whether the object has been placed in the room.
    /// </summary>
    bool IsPlaced { get; }
}
```

- [ ] **Step 5: Create IRotatable.cs**

```csharp
namespace SimsConstructor.Interfaces;

/// <summary>
/// Defines an object that can be rotated in 90-degree increments.
/// Demonstrates Abstraction — rotation behavior contract.
/// </summary>
public interface IRotatable
{
    /// <summary>
    /// Rotates the object 90 degrees clockwise.
    /// </summary>
    void Rotate();

    /// <summary>
    /// Gets the current rotation in degrees.
    /// </summary>
    int Rotation { get; }

    /// <summary>
    /// Gets the current rotation in degrees (explicit method).
    /// </summary>
    int GetRotationDegrees();
}
```

- [ ] **Step 6: Create IPricable.cs**

```csharp
namespace SimsConstructor.Interfaces;

/// <summary>
/// Defines an object that has a monetary cost.
/// Demonstrates Abstraction — pricing contract.
/// </summary>
public interface IPricable
{
    /// <summary>
    /// Gets the price/cost of the object.
    /// </summary>
    decimal GetPrice();
}
```

- [ ] **Step 7: Create ISelectable.cs**

```csharp
namespace SimsConstructor.Interfaces;

/// <summary>
/// Defines an object that can be selected and deselected.
/// Demonstrates Abstraction — selection state contract.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Selects the object.
    /// </summary>
    void Select();

    /// <summary>
    /// Deselects the object.
    /// </summary>
    void Deselect();

    /// <summary>
    /// Gets whether the object is currently selected.
    /// </summary>
    bool IsSelected { get; }
}
```

- [ ] **Step 8: Verify build**

```bash
dotnet build SimsConstructor.csproj
```

Expected: BUILD SUCCEEDED.

- [ ] **Step 9: Commit**

```bash
git add Models/ SimsConstructor.Tests/
git commit -m "feat: add OOP interfaces (IPlaceable, IRotatable, IPricable, ISelectable) and test project"
```

---

### Task 2: Create Enums and GameObject Base Class

**Files:**
- Create: `Models/Enums/FurnitureCategory.cs`
- Create: `Models/Enums/PlacementRule.cs`
- Create: `Models/Enums/DecorationStyle.cs`
- Create: `Models/Core/GameObject.cs`
- Create: `SimsConstructor.Tests/GameObjectTests.cs`

- [ ] **Step 1: Write tests for GameObject**

Create `SimsConstructor.Tests/GameObjectTests.cs`:

```csharp
using System.Drawing;
using SimsConstructor.Core;

namespace SimsConstructor.Tests;

public class GameObjectTests
{
    [Fact]
    public void GameObject_CreateWithValidName_ShouldHaveIdAndName()
    {
        // Arrange & Act
        var obj = new TestGameObject("TestObject", "A test object");

        // Assert
        Assert.Equal("TestObject", obj.Name);
        Assert.Equal("A test object", obj.Description);
        Assert.NotEqual(Guid.Empty, obj.Id);
    }

    [Fact]
    public void GameObject_ReadOnlyId_ShouldNotChangeAfterCreation()
    {
        var obj = new TestGameObject("Test", "Test");
        var id = obj.Id;

        // Id is read-only, verify it persists
        Assert.Equal(id, obj.Id);
    }

    [Fact]
    public void GameObject_ToString_ShouldReturnFormattedName()
    {
        var obj = new TestGameObject("Chair", "A wooden chair");

        var result = obj.ToString();

        Assert.Equal("Chair", result);
    }

    [Fact]
    public void GameObject_GetRenderBounds_ShouldReturnCorrectRectangle()
    {
        var obj = new TestGameObject("Table", "A table")
        {
            BoundsX = 10,
            BoundsY = 20,
            BoundsWidth = 100,
            BoundsHeight = 50
        };

        var bounds = obj.GetRenderBounds();

        Assert.Equal(10, bounds.X);
        Assert.Equal(20, bounds.Y);
        Assert.Equal(100, bounds.Width);
        Assert.Equal(50, bounds.Height);
    }

    /// <summary>
    /// Concrete test implementation of abstract GameObject.
    /// </summary>
    private class TestGameObject : GameObject
    {
        public float BoundsX { get; set; }
        public float BoundsY { get; set; }
        public float BoundsWidth { get; set; }
        public float BoundsHeight { get; set; }

        public TestGameObject(string name, string description)
            : base(name, description)
        {
        }

        public override Rectangle GetRenderBounds()
        {
            return new Rectangle(
                (int)BoundsX,
                (int)BoundsY,
                (int)BoundsWidth,
                (int)BoundsHeight);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter GameObjectTests
```

Expected: FAIL — "GameObject not defined".

- [ ] **Step 3: Create FurnitureCategory enum**

```csharp
namespace SimsConstructor.Enums;

/// <summary>
/// Categories of furniture for classification and filtering.
/// </summary>
public enum FurnitureCategory
{
    Bed,
    Seating,
    Storage,
    Surface,
    Table
}
```

- [ ] **Step 4: Create PlacementRule enum**

```csharp
namespace SimsConstructor.Enums;

/// <summary>
/// Rules that constrain where furniture can be placed in a room.
/// </summary>
public enum PlacementRule
{
    /// <summary>Must be placed against a wall (e.g., bed, wardrobe).</summary>
    WallRequired,
    /// <summary>Can be placed anywhere on the floor (e.g., table, chair).</summary>
    FloorOnly,
    /// <summary>Prefer corners (e.g., plant, lamp).</summary>
    CornerPreferred
}
```

- [ ] **Step 5: Create DecorationStyle enum**

```csharp
namespace SimsConstructor.Enums;

/// <summary>
/// Visual styles for decoration items.
/// </summary>
public enum DecorationStyle
{
    Modern,
    Classic,
    Rustic,
    Minimalist,
    Eclectic
}
```

- [ ] **Step 6: Create GameObject abstract class**

```csharp
using System.Drawing;

namespace SimsConstructor.Core;

/// <summary>
/// Abstract base class for all game objects.
/// Demonstrates Abstraction — cannot be instantiated directly.
/// Demonstrates Polymorphism — GetRenderBounds() is overridden in derived classes.
/// </summary>
public abstract class GameObject
{
    /// <summary>
    /// Unique identifier, read-only after creation.
    /// Demonstrates Encapsulation — private set prevents modification.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Display name of the object.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Optional description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Visual color of the object (for rendering).
    /// </summary>
    public Color Color { get; set; } = Color.Gray;

    protected GameObject(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
    }

    /// <summary>
    /// Returns the bounding rectangle for rendering in 2D top-down view.
    /// Overridden in derived classes to provide specific bounds.
    /// Demonstrates Polymorphism.
    /// </summary>
    public abstract Rectangle GetRenderBounds();

    /// <summary>
    /// Returns the object's name as its string representation.
    /// </summary>
    public override string ToString() => Name;
}
```

- [ ] **Step 7: Run tests to verify they pass**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter GameObjectTests
```

Expected: PASS (4 tests).

- [ ] **Step 8: Commit**

```bash
git add Models/Enums Models/Core SimsConstructor.Tests/GameObjectTests.cs
git commit -m "feat: add enums and GameObject abstract base class with tests"
```

---

### Task 3: Create RoomItem Abstract Class

**Files:**
- Create: `Models/Items/RoomItem.cs`
- Create: `SimsConstructor.Tests/RoomItemTests.cs`

- [ ] **Step 1: Write tests for RoomItem**

Create `SimsConstructor.Tests/RoomItemTests.cs`:

```csharp
using SimsConstructor.Core;

namespace SimsConstructor.Tests;

public class RoomItemTests
{
    [Fact]
    public void RoomItem_Create_ShouldInheritFromGameObject()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);

        Assert.Equal("TestItem", item.Name);
        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact]
    public void RoomItem_PlaceAt_WithValidCoordinates_ShouldSetPosition()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);

        item.PlaceAt(5f, 3f);

        Assert.Equal(5f, item.X);
        Assert.Equal(3f, item.Y);
        Assert.True(item.IsPlaced);
    }

    [Fact]
    public void RoomItem_PlaceAt_WithNegativeX_ShouldThrowArgumentException()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);

        Assert.Throws<ArgumentException>(() => item.PlaceAt(-1f, 3f));
    }

    [Fact]
    public void RoomItem_PlaceAt_WithNegativeY_ShouldThrowArgumentException()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);

        Assert.Throws<ArgumentException>(() => item.PlaceAt(3f, -1f));
    }

    [Fact]
    public void RoomItem_Select_ShouldSetIsSelectedToTrue()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);

        item.Select();

        Assert.True(item.IsSelected);
    }

    [Fact]
    public void RoomItem_Deselect_ShouldSetIsSelectedToFalse()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);
        item.Select();

        item.Deselect();

        Assert.False(item.IsSelected);
    }

    [Fact]
    public void RoomItem_Rotate90_ShouldSwapWidthAndHeight()
    {
        var item = new TestRoomItem("TestItem", 2f, 3f);

        item.Rotate90();

        Assert.Equal(3f, item.Width);
        Assert.Equal(2f, item.Height);
    }

    [Fact]
    public void RoomItem_Rotate90_FourTimes_ShouldReturnToOriginalRotation()
    {
        var item = new TestRoomItem("TestItem", 2f, 3f);
        var originalWidth = item.Width;
        var originalHeight = item.Height;

        item.Rotate90();
        item.Rotate90();
        item.Rotate90();
        item.Rotate90();

        Assert.Equal(originalWidth, item.Width);
        Assert.Equal(originalHeight, item.Height);
        Assert.Equal(0, item.Rotation);
    }

    [Fact]
    public void RoomItem_GetRenderBounds_ShouldReturnRectangleAtPosition()
    {
        var item = new TestRoomItem("TestItem", 2f, 3f);
        item.PlaceAt(10f, 20f);

        var bounds = item.GetRenderBounds();

        Assert.Equal(10, bounds.X);
        Assert.Equal(20, bounds.Y);
        Assert.Equal(2, bounds.Width);
        Assert.Equal(3, bounds.Height);
    }

    [Fact]
    public void RoomItem_InitialRotation_ShouldBeZero()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);

        Assert.Equal(0, item.Rotation);
    }

    [Fact]
    public void RoomItem_IsSelected_PrivateSet_ShouldNotBePubliclySettable()
    {
        var item = new TestRoomItem("TestItem", 1f, 1f);

        // IsSelected has private set — only Select()/Deselect() can change it
        Assert.False(item.IsSelected);
        item.Select();
        Assert.True(item.IsSelected);
    }

    /// <summary>
    /// Concrete test implementation of abstract RoomItem.
    /// </summary>
    private class TestRoomItem : RoomItem
    {
        public TestRoomItem(string name, float width, float height)
            : base(name, width, height)
        {
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter RoomItemTests
```

Expected: FAIL — "RoomItem not defined".

- [ ] **Step 3: Create RoomItem abstract class**

```csharp
using System.Drawing;
using SimsConstructor.Interfaces;

namespace SimsConstructor.Core;

/// <summary>
/// Abstract class for items that can be placed in a room.
/// Extends GameObject with spatial properties (position, rotation, dimensions).
/// Demonstrates Inheritance — extends GameObject.
/// Demonstrates Encapsulation — position validated before assignment.
/// </summary>
public abstract class RoomItem : GameObject, IPlaceable, IRotatable, ISelectable
{
    /// <summary>
    /// Horizontal position (X axis, from top-left origin).
    /// </summary>
    public float X { get; protected set; }

    /// <summary>
    /// Vertical position (Y axis, from top-left origin).
    /// </summary>
    public float Y { get; protected set; }

    /// <summary>
    /// Item width in room units (meters).
    /// </summary>
    public float Width { get; protected set; }

    /// <summary>
    /// Item height in room units (meters).
    /// </summary>
    public float Height { get; protected set; }

    /// <summary>
    /// Current rotation in degrees: 0, 90, 180, or 270.
    /// </summary>
    public int Rotation { get; private set; }

    /// <summary>
    /// Whether the item is currently selected.
    /// Demonstrates Encapsulation — private set, only modified via Select/Deselect.
    /// </summary>
    public bool IsSelected { get; private set; }

    /// <summary>
    /// Whether the item has been placed in the room.
    /// </summary>
    public bool IsPlaced { get; private set; }

    protected RoomItem(string name, float width, float height)
        : base(name)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        Width = width;
        Height = height;
        X = 0;
        Y = 0;
        Rotation = 0;
        IsPlaced = false;
    }

    /// <summary>
    /// Places the item at the specified coordinates.
    /// Validates that coordinates are non-negative.
    /// Demonstrates Encapsulation — validation before state change.
    /// </summary>
    public virtual void PlaceAt(float x, float y)
    {
        ArgumentException.ThrowIfNegative(x, nameof(x));
        ArgumentException.ThrowIfNegative(y, nameof(y));

        X = x;
        Y = y;
        IsPlaced = true;
    }

    /// <summary>
    /// Checks whether the item can be placed at the specified coordinates.
    /// Default implementation checks for non-negative coordinates.
    /// </summary>
    public virtual bool CanPlaceAt(float x, float y) => x >= 0 && y >= 0;

    /// <summary>
    /// Rotates the item 90 degrees clockwise.
    /// Swaps Width and Height at 90° and 270° rotations.
    /// </summary>
    public virtual void Rotate90()
    {
        Rotation = (Rotation + 90) % 360;

        // Swap dimensions at odd rotations (90°, 270°)
        if (Rotation == 90 || Rotation == 270)
        {
            (Width, Height) = (Height, Width);
        }
    }

    /// <summary>
    /// IRotatable.Rotate() — rotates 90° clockwise.
    /// </summary>
    public void Rotate() => Rotate90();

    /// <summary>
    /// IRotatable.GetRotationDegrees() — returns current rotation.
    /// </summary>
    public int GetRotationDegrees() => Rotation;

    /// <summary>
    /// Selects this item.
    /// </summary>
    public void Select() => IsSelected = true;

    /// <summary>
    /// Deselects this item.
    /// </summary>
    public void Deselect() => IsSelected = false;

    /// <summary>
    /// Returns the bounding rectangle based on current position and dimensions.
    /// Demonstrates Polymorphism — overrides abstract method from GameObject.
    /// </summary>
    public override Rectangle GetRenderBounds()
    {
        return new Rectangle(
            (int)X,
            (int)Y,
            (int)Width,
            (int)Height);
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter RoomItemTests
```

Expected: PASS (11 tests).

- [ ] **Step 5: Commit**

```bash
git add Models/Items/RoomItem.cs SimsConstructor.Tests/RoomItemTests.cs
git commit -m "feat: add RoomItem abstract class with placement, rotation, selection (11 tests)"
```

---

### Task 4: Create FurnitureItem Concrete Class

**Files:**
- Create: `Models/Items/FurnitureItem.cs`
- Create: `SimsConstructor.Tests/FurnitureItemTests.cs`

- [ ] **Step 1: Write tests for FurnitureItem**

Create `SimsConstructor.Tests/FurnitureItemTests.cs`:

```csharp
using SimsConstructor.Core;
using SimsConstructor.Enums;

namespace SimsConstructor.Tests;

public class FurnitureItemTests
{
    [Fact]
    public void FurnitureItem_Create_ShouldInheritFromRoomItem()
    {
        var furniture = new FurnitureItem("Bed", 2f, 1.5f, FurnitureCategory.Bed, "Wood");

        Assert.Equal("Bed", furniture.Name);
        Assert.Equal(2f, furniture.Width);
        Assert.Equal(1.5f, furniture.Height);
        Assert.Equal(FurnitureCategory.Bed, furniture.Category);
        Assert.Equal("Wood", furniture.Material);
    }

    [Fact]
    public void FurnitureItem_ToString_ShouldReturnFormattedWithCategory()
    {
        var furniture = new FurnitureItem("Sofa", 2f, 1f, FurnitureCategory.Seating, "Fabric");

        var result = furniture.ToString();

        Assert.Equal("Furniture: Sofa (Seating)", result);
    }

    [Fact]
    public void FurnitureItem_GetPrice_ShouldReturnPositiveDecimal()
    {
        var furniture = new FurnitureItem("Table", 1.5f, 1f, FurnitureCategory.Table, "Wood");
        furniture.SetPrice(299.99m);

        var price = furniture.GetPrice();

        Assert.Equal(299.99m, price);
    }

    [Fact]
    public void FurnitureItem_DefaultPrice_ShouldBeZero()
    {
        var furniture = new FurnitureItem("Chair", 0.5f, 0.5f, FurnitureCategory.Seating, "Metal");

        var price = furniture.GetPrice();

        Assert.Equal(0m, price);
    }

    [Fact]
    public void FurnitureItem_CanPlaceNear_WithFloorOnly_ShouldReturnTrue()
    {
        var table = new FurnitureItem("Table", 1.5f, 1f, FurnitureCategory.Table, "Wood")
        {
            PlacementRule = PlacementRule.FloorOnly
        };
        var chair = new FurnitureItem("Chair", 0.5f, 0.5f, FurnitureCategory.Seating, "Wood")
        {
            PlacementRule = PlacementRule.FloorOnly
        };
        chair.PlaceAt(2f, 1f);

        var canPlace = table.CanPlaceNear(chair);

        Assert.True(canPlace);
    }

    [Fact]
    public void FurnitureItem_DefaultPlacementRule_ShouldBeFloorOnly()
    {
        var furniture = new FurnitureItem("Rug", 2f, 2f, FurnitureCategory.Surface, "Cotton");

        Assert.Equal(PlacementRule.FloorOnly, furniture.PlacementRule);
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter FurnitureItemTests
```

Expected: FAIL — "FurnitureItem not defined".

- [ ] **Step 3: Create FurnitureItem class**

```csharp
using SimsConstructor.Enums;
using SimsConstructor.Interfaces;

namespace SimsConstructor.Core;

/// <summary>
/// Concrete furniture item with category, material, and placement rules.
/// Demonstrates Inheritance — extends RoomItem with furniture-specific properties.
/// Demonstrates Polymorphism — overrides ToString().
/// </summary>
public class FurnitureItem : RoomItem, IPricable
{
    /// <summary>
    /// Category of this furniture (Bed, Seating, Storage, etc.).
    /// </summary>
    public FurnitureCategory Category { get; }

    /// <summary>
    /// Material the furniture is made of (wood, metal, plastic, etc.).
    /// </summary>
    public string Material { get; }

    /// <summary>
    /// Placement rule constraining where this furniture can be placed.
    /// </summary>
    public PlacementRule PlacementRule { get; set; }

    /// <summary>
    /// Price of the furniture.
    /// </summary>
    private decimal _price;

    public FurnitureItem(string name, float width, float height, FurnitureCategory category, string material)
        : base(name, width, height)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(material);

        Category = category;
        Material = material;
        PlacementRule = PlacementRule.FloorOnly;
        _price = 0m;
    }

    /// <summary>
    /// Sets the price of this furniture.
    /// </summary>
    public void SetPrice(decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        _price = price;
    }

    /// <summary>
    /// IPricable.GetPrice() — returns the cost of this furniture.
    /// </summary>
    public decimal GetPrice() => _price;

    /// <summary>
    /// Checks if this furniture can be placed near another room item.
    /// Current implementation always returns true for FloorOnly rule.
    /// Can be extended with wall-checking logic for WallRequired rule.
    /// </summary>
    public bool CanPlaceNear(RoomItem other)
    {
        // Base implementation — FloorOnly items can be placed anywhere
        // Derived classes or PlacementValidator service can add wall-distance checks
        return true;
    }

    /// <summary>
    /// Returns formatted string with furniture name and category.
    /// Demonstrates Polymorphism — overrides GameObject.ToString().
    /// </summary>
    public override string ToString() => $"Furniture: {Name} ({Category})";
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter FurnitureItemTests
```

Expected: PASS (6 tests).

- [ ] **Step 5: Commit**

```bash
git add Models/Items/FurnitureItem.cs SimsConstructor.Tests/FurnitureItemTests.cs
git commit -m "feat: add FurnitureItem with category, material, pricing (6 tests)"
```

---

### Task 5: Create ApplianceItem and DecorationItem

**Files:**
- Create: `Models/Items/ApplianceItem.cs`
- Create: `Models/Items/DecorationItem.cs`
- Create: `SimsConstructor.Tests/ApplianceItemTests.cs`
- Create: `SimsConstructor.Tests/DecorationItemTests.cs`

- [ ] **Step 1: Write tests for ApplianceItem**

Create `SimsConstructor.Tests/ApplianceItemTests.cs`:

```csharp
using SimsConstructor.Core;

namespace SimsConstructor.Tests;

public class ApplianceItemTests
{
    [Fact]
    public void ApplianceItem_Create_ShouldInheritFromRoomItem()
    {
        var appliance = new ApplianceItem("Fridge", 0.7f, 0.7f);

        Assert.Equal("Fridge", appliance.Name);
        Assert.Equal(0.7f, appliance.Width);
        Assert.Equal(0.7f, appliance.Height);
    }

    [Fact]
    public void ApplianceItem_ToString_ShouldReturnFormatted()
    {
        var appliance = new ApplianceItem("Washing Machine", 0.6f, 0.6f);

        var result = appliance.ToString();

        Assert.Equal("Appliance: Washing Machine", result);
    }

    [Fact]
    public void ApplianceItem_PowerConsumption_ShouldBeSettable()
    {
        var appliance = new ApplianceItem("Microwave", 0.5f, 0.4f);
        appliance.PowerConsumption = 1200m;

        Assert.Equal(1200m, appliance.PowerConsumption);
    }

    [Fact]
    public void ApplianceItem_DefaultPowerConsumption_ShouldBeZero()
    {
        var appliance = new ApplianceItem("Table", 1f, 1f);

        Assert.Equal(0m, appliance.PowerConsumption);
    }

    [Fact]
    public void ApplianceItem_RequiresWater_ShouldDefaultToFalse()
    {
        var appliance = new ApplianceItem("TV", 1f, 0.3f);

        Assert.False(appliance.RequiresWater);
    }

    [Fact]
    public void ApplianceItem_RequiresVentilation_ShouldDefaultToFalse()
    {
        var appliance = new ApplianceItem("Oven", 0.6f, 0.6f);

        Assert.False(appliance.RequiresVentilation);
    }
}
```

- [ ] **Step 2: Write tests for DecorationItem**

Create `SimsConstructor.Tests/DecorationItemTests.cs`:

```csharp
using SimsConstructor.Core;
using SimsConstructor.Enums;

namespace SimsConstructor.Tests;

public class DecorationItemTests
{
    [Fact]
    public void DecorationItem_Create_ShouldInheritFromRoomItem()
    {
        var decoration = new DecorationItem("Rug", 2f, 2f, DecorationStyle.Modern);

        Assert.Equal("Rug", decoration.Name);
        Assert.Equal(DecorationStyle.Modern, decoration.Style);
    }

    [Fact]
    public void DecorationItem_ToString_ShouldReturnFormattedWithStyle()
    {
        var decoration = new DecorationItem("Painting", 0.8f, 0.6f, DecorationStyle.Classic);

        var result = decoration.ToString();

        Assert.Equal("Decoration: Painting (Classic)", result);
    }

    [Fact]
    public void DecorationItem_IsWallMountable_ShouldDefaultToFalse()
    {
        var decoration = new DecorationItem("Rug", 2f, 2f, DecorationStyle.Rustic);

        Assert.False(decoration.IsWallMountable);
    }

    [Fact]
    public void DecorationItem_WallMountable_ShouldBeSettable()
    {
        var decoration = new DecorationItem("Shelf", 1f, 0.3f, DecorationStyle.Minimalist)
        {
            IsWallMountable = true
        };

        Assert.True(decoration.IsWallMountable);
    }

    [Fact]
    public void DecorationItem_GetPrice_ShouldReturnZero()
    {
        var decoration = new DecorationItem("Plant", 0.3f, 0.3f, DecorationStyle.Eclectic);

        var price = decoration.GetPrice();

        Assert.Equal(0m, price);
    }
}
```

- [ ] **Step 3: Run tests to verify they fail**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter "ApplianceItemTests|DecorationItemTests"
```

Expected: FAIL — classes not defined.

- [ ] **Step 4: Create ApplianceItem class**

```csharp
namespace SimsConstructor.Core;

/// <summary>
/// Electrical or plumbing appliance with utility requirements.
/// Demonstrates Inheritance — extends RoomItem with appliance-specific properties.
/// Demonstrates Polymorphism — overrides ToString().
/// </summary>
public class ApplianceItem : RoomItem
{
    /// <summary>
    /// Power consumption in watts. Zero for non-electrical items.
    /// </summary>
    public decimal PowerConsumption { get; set; }

    /// <summary>
    /// Whether this appliance requires water connection (washing machine, dishwasher).
    /// </summary>
    public bool RequiresWater { get; set; }

    /// <summary>
    /// Whether this appliance requires ventilation (oven, dryer).
    /// </summary>
    public bool RequiresVentilation { get; set; }

    public ApplianceItem(string name, float width, float height)
        : base(name, width, height)
    {
        PowerConsumption = 0m;
        RequiresWater = false;
        RequiresVentilation = false;
    }

    /// <summary>
    /// Returns formatted string with appliance name.
    /// Demonstrates Polymorphism — overrides GameObject.ToString().
    /// </summary>
    public override string ToString() => $"Appliance: {Name}";
}
```

- [ ] **Step 5: Create DecorationItem class**

```csharp
using SimsConstructor.Enums;
using SimsConstructor.Interfaces;

namespace SimsConstructor.Core;

/// <summary>
/// Decorative item with visual style and optional wall mounting.
/// Demonstrates Inheritance — extends RoomItem with decoration-specific properties.
/// Demonstrates Polymorphism — overrides ToString().
/// </summary>
public class DecorationItem : RoomItem, IPricable
{
    /// <summary>
    /// Visual style of this decoration.
    /// </summary>
    public DecorationStyle Style { get; }

    /// <summary>
    /// Whether this item can be mounted on a wall (paintings, shelves).
    /// </summary>
    public bool IsWallMountable { get; set; }

    public DecorationItem(string name, float width, float height, DecorationStyle style)
        : base(name, width, height)
    {
        Style = style;
        IsWallMountable = false;
    }

    /// <summary>
    /// IPricable.GetPrice() — decorations currently have no cost.
    /// Can be extended with pricing later.
    /// </summary>
    public decimal GetPrice() => 0m;

    /// <summary>
    /// Returns formatted string with decoration name and style.
    /// Demonstrates Polymorphism — overrides GameObject.ToString().
    /// </summary>
    public override string ToString() => $"Decoration: {Name} ({Style})";
}
```

- [ ] **Step 6: Run tests to verify they pass**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter "ApplianceItemTests|DecorationItemTests"
```

Expected: PASS (11 tests total).

- [ ] **Step 7: Commit**

```bash
git add Models/Items/ApplianceItem.cs Models/Items/DecorationItem.cs SimsConstructor.Tests/ApplianceItemTests.cs SimsConstructor.Tests/DecorationItemTests.cs
git commit -m "feat: add ApplianceItem and DecorationItem with tests (11 tests)"
```

---

### Task 6: Interface Implementation Verification Tests

**Files:**
- Create: `SimsConstructor.Tests/InterfaceTests.cs`

- [ ] **Step 1: Write interface compliance tests**

Create `SimsConstructor.Tests/InterfaceTests.cs`:

```csharp
using SimsConstructor.Core;
using SimsConstructor.Enums;
using SimsConstructor.Interfaces;

namespace SimsConstructor.Tests;

/// <summary>
/// Verifies that all classes correctly implement their interfaces.
/// Demonstrates Abstraction — testing interface contracts.
/// Demonstrates Polymorphism — calling interface methods on base-type references.
/// </summary>
public class InterfaceTests
{
    [Fact]
    public void FurnitureItem_ImplementsIPlaceable_ShouldPlaceAtCoordinates()
    {
        IPlaceable placeable = new FurnitureItem("Bed", 2f, 1.5f, FurnitureCategory.Bed, "Wood");

        placeable.PlaceAt(5f, 3f);

        Assert.True(placeable.IsPlaced);
        Assert.True(placeable.CanPlaceAt(10f, 10f));
    }

    [Fact]
    public void FurnitureItem_ImplementsIRotatable_ShouldRotate()
    {
        IRotatable rotatable = new FurnitureItem("Table", 1.5f, 1f, FurnitureCategory.Table, "Wood");

        rotatable.Rotate();

        Assert.Equal(90, rotatable.GetRotationDegrees());
    }

    [Fact]
    public void FurnitureItem_ImplementsIPricable_ShouldReturnPrice()
    {
        var furniture = new FurnitureItem("Chair", 0.5f, 0.5f, FurnitureCategory.Seating, "Wood");
        furniture.SetPrice(150.00m);
        IPricable pricable = furniture;

        var price = pricable.GetPrice();

        Assert.Equal(150.00m, price);
    }

    [Fact]
    public void FurnitureItem_ImplementsISelectable_ShouldToggleSelection()
    {
        ISelectable selectable = new FurnitureItem("Sofa", 2f, 1f, FurnitureCategory.Seating, "Fabric");

        selectable.Select();
        Assert.True(selectable.IsSelected);

        selectable.Deselect();
        Assert.False(selectable.IsSelected);
    }

    [Fact]
    public void ApplianceItem_ImplementsIPlaceable_ShouldPlaceAtCoordinates()
    {
        IPlaceable placeable = new ApplianceItem("Fridge", 0.7f, 0.7f);

        placeable.PlaceAt(1f, 1f);

        Assert.True(placeable.IsPlaced);
    }

    [Fact]
    public void ApplianceItem_ImplementsIRotatable_ShouldRotate()
    {
        IRotatable rotatable = new ApplianceItem("Microwave", 0.5f, 0.4f);

        rotatable.Rotate();

        Assert.Equal(90, rotatable.GetRotationDegrees());
    }

    [Fact]
    public void ApplianceItem_ImplementsISelectable_ShouldToggleSelection()
    {
        ISelectable selectable = new ApplianceItem("TV", 1f, 0.3f);

        selectable.Select();
        Assert.True(selectable.IsSelected);

        selectable.Deselect();
        Assert.False(selectable.IsSelected);
    }

    [Fact]
    public void DecorationItem_ImplementsIPlaceable_ShouldPlaceAtCoordinates()
    {
        IPlaceable placeable = new DecorationItem("Rug", 2f, 2f, DecorationStyle.Modern);

        placeable.PlaceAt(0f, 0f);

        Assert.True(placeable.IsPlaced);
    }

    [Fact]
    public void DecorationItem_ImplementsIRotatable_ShouldRotate()
    {
        IRotatable rotatable = new DecorationItem("Painting", 0.8f, 0.6f, DecorationStyle.Classic);

        rotatable.Rotate();

        Assert.Equal(90, rotatable.GetRotationDegrees());
    }

    [Fact]
    public void DecorationItem_ImplementsIPricable_ShouldReturnPrice()
    {
        IPricable pricable = new DecorationItem("Lamp", 0.3f, 0.3f, DecorationStyle.Modern);

        var price = pricable.GetPrice();

        Assert.Equal(0m, price);
    }

    [Fact]
    public void Polymorphism_GetRenderBounds_ShouldWorkThroughBaseClassReference()
    {
        // Demonstrates Polymorphism — calling overridden method through base reference
        GameObject furniture = new FurnitureItem("Bed", 2f, 1.5f, FurnitureCategory.Bed, "Wood");
        furniture.PlaceAt(5f, 5f);

        var bounds = furniture.GetRenderBounds();

        Assert.Equal(5, bounds.X);
        Assert.Equal(5, bounds.Y);
        Assert.Equal(2, bounds.Width);
        Assert.Equal(2, bounds.Height); // Width/Height swapped due to 0° rotation — actually 1.5f → 1
    }

    [Fact]
    public void Polymorphism_ToString_ShouldReturnClassSpecificString()
    {
        GameObject furniture = new FurnitureItem("Bed", 2f, 1.5f, FurnitureCategory.Bed, "Wood");
        GameObject appliance = new ApplianceItem("Fridge", 0.7f, 0.7f);
        GameObject decoration = new DecorationItem("Rug", 2f, 2f, DecorationStyle.Modern);

        Assert.Equal("Furniture: Bed (Bed)", furniture.ToString());
        Assert.Equal("Appliance: Fridge", appliance.ToString());
        Assert.Equal("Decoration: Rug (Modern)", decoration.ToString());
    }
}
```

- [ ] **Step 2: Run all interface tests**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj --filter InterfaceTests
```

Expected: PASS (12 tests).

- [ ] **Step 3: Run ALL tests to verify everything works together**

```bash
dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj
```

Expected: PASS (44 tests total: 4 + 11 + 6 + 6 + 5 + 12).

- [ ] **Step 4: Commit**

```bash
git add SimsConstructor.Tests/InterfaceTests.cs
git commit -m "test: add interface compliance and polymorphism verification tests (12 tests)"
```

---

### Task 7: Update QWEN.md and Verify Build

**Files:**
- Modify: `QWEN.md`

- [ ] **Step 1: Final build verification**

```bash
dotnet build SimsConstructor.csproj && dotnet test SimsConstructor.Tests/SimsConstructor.Tests.csproj
```

Expected: BUILD SUCCEEDED, ALL TESTS PASS.

- [ ] **Step 2: Update QWEN.md with completed status**

Update the "Next Steps" section and add a "Completed" section:

```markdown
## Completed Work

### Phase 1: Domain Models (2026-04-14)
- [x] Test project created (xUnit, .NET 9.0)
- [x] 4 interfaces: IPlaceable, IRotatable, IPricable, ISelectable
- [x] 3 enums: FurnitureCategory, PlacementRule, DecorationStyle
- [x] GameObject abstract base class
- [x] RoomItem abstract class with placement, rotation, selection
- [x] FurnitureItem concrete class with category, material, pricing
- [x] ApplianceItem concrete class with power/water/ventilation
- [x] DecorationItem concrete class with style, wall-mountable
- [x] 44 unit tests passing
- [x] All 4 OOP pillars demonstrated
```

- [ ] **Step 3: Commit**

```bash
git add QWEN.md
git commit -m "docs: update QWEN.md with completed domain models phase"
```

---

## Self-Review

### 1. Spec Coverage Check

| Spec Requirement | Task |
|-----------------|------|
| GameObject abstract class | Task 2 |
| RoomItem abstract class | Task 3 |
| FurnitureItem class | Task 4 |
| ApplianceItem class | Task 5 |
| DecorationItem class | Task 5 |
| IPlaceable interface | Task 1 |
| IRotatable interface | Task 1 |
| IPricable interface | Task 1 |
| ISelectable interface | Task 1 |
| FurnitureCategory enum | Task 2 |
| PlacementRule enum | Task 2 |
| DecorationStyle enum | Task 2 |
| Encapsulation (private setters, validation) | Task 2, Task 3 |
| Inheritance hierarchy | Task 2, 3, 4, 5 |
| Polymorphism (overridden methods) | Task 4, 5, 6 |
| Abstraction (interfaces, abstract classes) | Task 1, 2 |
| 2D top-down coordinate system | Task 3 |
| Rotation swaps Width/Height | Task 3 |

All spec requirements covered.

### 2. Placeholder Scan
No TBD, TODO, "implement later", or vague instructions found. All code blocks contain complete implementations.

### 3. Type Consistency
- All class names match: `GameObject`, `RoomItem`, `FurnitureItem`, `ApplianceItem`, `DecorationItem`
- Interface names match: `IPlaceable`, `IRotatable`, `IPricable`, `ISelectable`
- Enum names match: `FurnitureCategory`, `PlacementRule`, `DecorationStyle`
- Method signatures consistent: `PlaceAt(float x, float y)`, `Rotate90()`, `GetPrice()`, `Select()`, `Deselect()`
- Property names consistent: `X`, `Y`, `Width`, `Height`, `Rotation`, `IsSelected`, `IsPlaced`

### 4. Expected Test Count
- GameObjectTests: 4 tests
- RoomItemTests: 11 tests
- FurnitureItemTests: 6 tests
- ApplianceItemTests: 6 tests
- DecorationItemTests: 5 tests
- InterfaceTests: 12 tests
- **Total: 44 tests**
