using Godot;
using System;

public abstract partial class EnemyEntity : Node2D
{
	[Export]
	public int Health {get; private set; }
	[Export]
	public bool isKilled {get; private set; }
	public EnemyEntity(int health)
	{
		this.Health = health;
	}

	protected abstract void onKilled();

	protected void TakeHit()
	{
		if(!isKilled) {
			this.Health--;
			if(this.Health < 0)
			{
				this.isKilled = true;
				onKilled();
			}
		}
	}
}
