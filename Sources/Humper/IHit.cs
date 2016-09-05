namespace Humper
{
	using Base;

	/// <summary>
	/// Represents a hit point out of a collision.
	/// </summary>
	public interface IHit
	{
		/// <summary>
		/// Gets the collided box.
		/// </summary>
		/// <value>The box.</value>
		IBox Box { get; }

		/// <summary>
		/// Gets the normal vector of the collided box side.
		/// </summary>
		/// <value>The normal.</value>
		Vector2 Normal { get;  }

		/// <summary>
		/// Gets the amount of movement needed from origin to get the impact position.
		/// </summary>
		/// <value>The amount.</value>
		float Amount { get; }

		/// <summary>
		/// Gets the impact position.
		/// </summary>
		/// <value>The position.</value>
		Vector2 Position { get;  }

		/// <summary>
		/// Gets the amount of movement needed from impact position to get the requested initial goal position.
		/// </summary>
		/// <value>The remaining value.</value>
		float Remaining { get; }

		/// <summary>
		/// Indicates whether the hit point is nearer than an other from a given point. Warning: this should only be used
		/// for multiple calculation of the same box movement (amount is compared first, then distance).
		/// </summary>
		/// <returns><c>true</c>, if nearest was ised, <c>false</c> otherwise.</returns>
		/// <param name="than">Than.</param>
		/// <param name="from">From.</param>
		bool IsNearest(IHit than, Vector2 from);
	}
}

