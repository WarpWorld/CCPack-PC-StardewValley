using System;
using ConnectorLib.JSON;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ControlValley;

public abstract class Behavior(ModEntry mod) : IEquatable<Behavior>
{
    public Guid ID { get; } = Guid.NewGuid();

    public Game1 Game => GameRunner.instance.gamePtr;

    public Farmer? Player => Game1.player;

    public Rectangle? ViewportBounds => Game?.GraphicsDevice?.Viewport.Bounds;

    public SpriteFont DialogueFont => Game1.dialogueFont;
    public SpriteFont SmallFont => Game1.smallFont;
    public SpriteFont TingFont => Game1.tinyFont;

    public ModEntry Mod { get; set; } = mod;

    public bool Active { get; set; } = true;

    public bool IsInitialized { get; private set; }
    public virtual void Initialize() => IsInitialized = true;

    public bool ContentLoaded { get; private set; }
    public virtual void LoadContent() => ContentLoaded = true;

    public virtual void Start(EffectRequest request)
    {
        if (!IsInitialized) Initialize();
        if (!ContentLoaded) LoadContent();
    }

    public virtual bool TryStop() => Mod.ActiveBehaviors.TryRemove(ID, out _);

    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }

    public bool Equals(Behavior other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID.Equals(other.ID);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Behavior)obj);
    }

    public override int GetHashCode() => ID.GetHashCode();
}