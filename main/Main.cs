using Godot;
using System;

public class Main : Node {

  [Export]
  public PackedScene Mob;

  private int _score;

  private Random _random = new Random();

  public override void _Ready() {
    GetNode<Player>("Player").Connect("Hit", this, nameof(GameOver));
    GetNode<Timer>("StartTimer").Connect("timeout", this, nameof(OnStartTimerTimeout));
    GetNode<Timer>("MobTimer").Connect("timeout", this, nameof(OnStartTimerTimeout));
    GetNode<Timer>("MobTimer").Connect("timeout", this, nameof(OnMobTimerTimeout));
    GetNode<Timer>("ScoreTimer").Connect("timeout", this, nameof(OnScoreTimerTimeout));

    NewGame();
  }

  public void NewGame() {
    _score = 0;

    Player player = GetNode<Player>("Player");
    Position2D startposition = GetNode<Position2D>("StartPosition");
    player.Start(startposition.Position);

    GetNode<Timer>("StartTimer").Start();
  }

  public void GameOver() {
    GetNode<Timer>("MobTimer").Stop();
    GetNode<Timer>("ScoreTimer").Stop();
  }

  public void OnStartTimerTimeout() {
    GetNode<Timer>("MobTimer").Start();
    GetNode<Timer>("ScoreTimer").Start();
  }

  public void OnScoreTimerTimeout() {
    _score++;
  }

  public void OnMobTimerTimeout() {
    /* Choose a random location on Path2D */
    PathFollow2D mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
    mobSpawnLocation.SetOffset(_random.Next());

    /* Create a Mob instance and add it to the scene */
    RigidBody2D mob = (RigidBody2D)Mob.Instance();
    AddChild(mob);

    /* Set mob's direction orthogonal to the path direction */
    float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

    /* Set the mob's position to a random location */
    mob.Position = mobSpawnLocation.Position;

    /* Add some randomness to the direction */
    direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
    mob.Rotation = direction;

    /* Choose velocity */
    mob.SetLinearVelocity(new Vector2(RandRange(150f, 250f), 0).Rotated(direction));
  }

  private float RandRange(float min, float max) {
    return (float)_random.NextDouble() * (max - min) + min;
  }

}
