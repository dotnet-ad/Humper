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

		Movement Move(float x, float y, Func<ICollision, ICollisionResponse> filter);

		Movement Move(float x, float y, Func<ICollision, CollisionResponses> filter);

		object Data { get; set; }

		#region Tags

		IBox AddTags(params Enum[] newTags);

		IBox RemoveTags(params Enum[] newTags);

		bool HasTag(params Enum[] values);

		bool HasTags(params Enum[] values);

		#endregion
	}
}

