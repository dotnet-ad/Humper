# Humper

Collision detection only for **Axis-Aligned-Bounding-Boxes** (*aka AABB*).

This library isn't a fully featured physics engine : don't expect a realistic physics simulation but it could be enough for basic game physics (platformer games, top-down games, shoot-em-ups). 

The library doesn't rely on any particular framework, its pure C# with all needed types included. It is fairly easy to integrate with existing frameworks like [Monogame](http://www.monogame.net/).

## Install

Available soon on NuGet

[![NuGet](https://img.shields.io/nuget/v/Humper.svg?label=NuGet)](https://www.nuget.org/packages/Humper/)

## Quickstart

![Schema](./Documentation/Quickstart.png)

```csharp
var world = new World(500, 300);

// Create a box of size (20,20) at position (100,100)
var body = world.Create(100, 100, 20, 20);

// Create a second box
world.Create(50, 150, 100, 100);

// Try to move the box to (100,200) with a slide movement for every other collided body of the world
var result = body.Move(170,200, (collision) => CollisionResponses.Slide);

// React to collisions
if(result.HasCollided)
{
	Debug.WriteLine("Body collided!");
}
```

## Basic APIs

### World

The world is a virtual representation of your physics environnement. It manages all bodies and have a given size.

The world is subdivided in cells of `64` by default to faster collision calculation, but you can also change this parameter at instanciation.

#### `IBox Create(float x, float y, float width, float height)`

Creates a new box with given coordinates and size.

#### `bool Remove(IBox box)`

Removes a box from the world.

#### `IBox Find(float x, float y, float width, float height)`

Queries the world to find all boxes in a given area.

### Box

A box represents any object of your physical world as an Axis-Aligned-Bounding-Boxes (*a rectangle that cannot be rotated*).

#### `float X { get; }`

The top left corner X coordinate of the box.

#### `float Y { get; }`

The top left corner Y coordinate of the box.

#### `float Height { get; }`

The height of the box.

#### `float Width { get; }`

The width of the box.

#### `IMovement Move(float x, float y, Func<ICollision, CollisionResponses> filter)`

Triggers a movement of the box in the physical world from its current position to the given one. The filters should indicate how the box reacts when colliding with another box of the world (see `Responses` section for more info).

#### `IBox AddTags(params Enum[] newTags)`

Add enumeration flags to be categorized.

#### `bool HasTag(params Enum[] values)`

Indicates whether the box has one of the given tags.

#### `bool HasTags(params Enum[] values)`

Indicates whether the box has all of the given tags.

#### `object Data { get; set; }`

Custom user data that can be attached to the box.

### Collision

A collision represents the result of a movement query that resulted in a collision.

#### `IBox Box { get; set; }`

The box that moved.

#### `IBox Other { get; set; }`

The box that have collided with the moving box.

#### `RectangleF Origin { get; set; }`

The starting position of the moving box.

#### `RectangleF Goal { get; set; }`

The queried destination.

#### `Hit Hit { get; set; }`

Information about the collision point.

#### `bool HasCollided { get; }`

Indicates whether this collision is valid.

### Hit

An hit point represents the impact of one box with an other.

#### `Vector2 Normal { get; set; }`

The normal vector of the collided side.

#### `float Amount { get; set; }`

The amount of movement accomplished from origin to impact, compared to goal destination.

#### `RectangleF Position { get; set; }`

The impact position of the box.

#### `float Remaining { get; }`

The remaining amount of movement to go from the impact to goal destination.

### Responses

When moving a box, a response should be returned through a filter to indicate how this box should react to a collision with another box.

Several responses are included :

* `Touch`: the box moves to the collision impact position.
* `Cross`: the box moves through and ignore collision.
* `Slide`: the box slides on the collided side of the other box.
* `Bounce`: the box is reflected from the side of the other box.

A custom implementation of `ICollisionResponse` can also be provided if needed.

### Debug layer

A debug layer is provided if you want to draw the boxes : you only have to provide basic drawing functions to the world `DrawDebug` method.

An example [Monogame](http://www.monogame.net/) implementation : 

```csharp
private void DrawCell(int x, int y, int w, int h, float alpha)
{
	spriteBatch.Draw(pixelTexture, pixelTexture.Bounds, new Rectangle(x,y,w,h), new Color(Color.White,alpha));
}

private void DrawBox(IBox box)
{
	spriteBatch.Draw(pixelTexture, pixelTexture.Bounds, box.Bounds.ToRectangle(), Color.Green);
}

private void DrawString(string message, int x, int y, float alpha)
{
	var size = this.font.MeasureString(message);
	spriteBatch.DrawString(this.font, message, new Vector2( x - size.X / 2, y - size.Y / 2), new Color(Color.White, alpha));
}
```

```csharp
protected override void Draw(GameTime gameTime)
{
	graphics.GraphicsDevice.Clear(Color.Black);
	spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
	var b = world.Bounds;
	world.DrawDebug((int)b.X, (int)b.Y, (int)b.Width, (int)b.Height, DrawCell, DrawBox, DrawString);
	spriteBatch.End();
	base.Draw(gameTime);
}
```

## Samples

Check the samples if you wish to implement a :

* `Top-down`
* `Platformer`

## Ideas / Roadmap

* Add resizing
* Improve documentation
* Optimize code

## Thanks

* [gamedev.net-SweptAABB](http://www.gamedev.net/page/resources/_/technical/game-programming/swept-aabb-collision-detection-and-response-r3084)
* [bump.lua](https://github.com/kikito/bump.lua)

## Contributions

Contributions are welcome! If you find a bug please report it and if you want a feature please report it.

If you want to contribute code please file an issue and create a branch off of the current dev branch and file a pull request.

### License

MIT © [Aloïs Deniel](http://aloisdeniel.github.io)