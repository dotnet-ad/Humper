namespace Humper
{
	using System;
	using Base;
	using Responses;

	public interface IBox
	{
		float X { get; }

		float Y { get; }

		float Width { get; }

		float Height { get; }

		RectangleF Bounds { get; }

		IMovement Move(float x, float y, Func<ICollision, ICollisionResponse> filter);

		IMovement Move(float x, float y, Func<ICollision, CollisionResponses> filter);

		object Data { get; set; }

		#region Tags

		IBox AddTags(params Enum[] newTags);

		IBox RemoveTags(params Enum[] newTags);

		bool HasTag(params Enum[] values);

		bool HasTags(params Enum[] values);

		#endregion
	}
}

