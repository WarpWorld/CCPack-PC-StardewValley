using System;
using System.Collections.Generic;
using ConnectorLib.JSON;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ControlValley.Effects;

// ReSharper disable once UnusedMember.Global
public class EffectHypeTrain(ModEntry mod) : Behavior(mod)
{
    private Vector2 m_trainPosition;
    private Vector2 m_trainVelocity;
    private float m_trainDirection;

    private readonly Dictionary<string, HypeTrainSourceDetails.Contribution> m_contributions = new();

    private static Texture2D? m_texTrainFront;
    private static Texture2D? m_texTrainBox;
    private static Texture2D? m_texTrainCoal;
    private static Texture2D? m_texTrainTank;

    private void TryGetTrainTextures(GraphicsDevice graphicsDevice)
    {
        m_texTrainFront ??= graphicsDevice.LoadEmbeddedTexture("Content\\HypeTrain\\front2_64.png");
        m_texTrainBox ??= graphicsDevice.LoadEmbeddedTexture("Content\\HypeTrain\\box_64.png");
        m_texTrainCoal ??= graphicsDevice.LoadEmbeddedTexture("Content\\HypeTrain\\coal_64.png");
        m_texTrainTank ??= graphicsDevice.LoadEmbeddedTexture("Content\\HypeTrain\\tank_64.png");
    }

    public override void LoadContent()
    {
        base.LoadContent();
        TryGetTrainTextures(Game.GraphicsDevice);
    }

    public override void Start(EffectRequest request)
    {
        base.Start(request);
        if (!Active || (Player == null)) return;

        if (!ViewportBounds.HasValue)
        {
            Mod.Client.Respond(request, EffectStatus.Retry, "Game camera is not ready.");
            TryStop();
            return;
        }
        if (m_texTrainFront == null) return;

        m_trainPosition = new Vector2(ViewportBounds.Value.Left - m_texTrainFront.Width, ViewportBounds.Value.Center.Y);
        m_trainVelocity = new Vector2(Math.Min(80, Math.Max(320, 32f * ((request?.sourceDetails as HypeTrainSourceDetails)?.Level ?? 1f))), 0f);
        m_trainDirection = Math.Sign(m_trainVelocity.X);

        m_contributions.Clear();
        // ReSharper disable RedundantAlwaysMatchSubpattern
        {
            if (request?.sourceDetails is HypeTrainSourceDetails { TopContributions: not null } sourceDetails)
                foreach (HypeTrainSourceDetails.Contribution contribution in sourceDetails.TopContributions)
                    m_contributions[contribution.UserName] = contribution;
        }
        {
            if (request?.sourceDetails is HypeTrainSourceDetails { LastContribution: not null } sourceDetails)
                    m_contributions[sourceDetails.LastContribution.UserName] = sourceDetails.LastContribution;
        }
        // ReSharper restore RedundantAlwaysMatchSubpattern
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!Active || (Player == null)) return;

        Rectangle? cameraBounds = ViewportBounds;
        if (!cameraBounds.HasValue) return;

        m_trainPosition += m_trainVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        //if (TrainPosition.X > cameraBounds.Value.Right)
            //TryStop();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        if (!Active || (Player == null)) return;

        if (m_texTrainFront == null) return;
        SpriteEffects effects = (m_trainDirection > 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        
        Texture2D tex = m_texTrainFront;
        spriteBatch.Draw(m_texTrainFront, new Rectangle((int)m_trainPosition.X, (int)m_trainPosition.Y, m_texTrainFront.Width, m_texTrainFront.Height), null, Color.White, 0f, Vector2.Zero, effects, 1f);
        
        float nextOffsetX = m_trainPosition.X + ((m_trainDirection < 0) ? m_texTrainFront.Width : 0f);
        foreach (var contribution in m_contributions)
        {
            tex = (string.Equals(contribution.Value.Type, "bits") ? m_texTrainCoal : m_texTrainBox)!;
            nextOffsetX += tex.Width * -m_trainDirection;
            float nextOffsetY;
            if (tex == m_texTrainCoal)
                nextOffsetY = -4f;
            else if (tex == m_texTrainBox)
                nextOffsetY = -4f;
            else if (tex == m_texTrainTank)
                nextOffsetY = -5f;
            else
                nextOffsetY = 0f;

            spriteBatch.Draw(tex, new Rectangle((int)nextOffsetX, (int)(m_trainPosition.Y + nextOffsetY), tex.Width, tex.Height), null, Color.White, 0f, Vector2.Zero, effects, 1f);
            
            string carText = contribution.Value.UserName;
            Vector2 halfText = SmallFont.MeasureString(carText) / 2f;
            spriteBatch.DrawString(SmallFont,
                carText,
                new Vector2(nextOffsetX + ((tex.Width * m_trainDirection) / 2f) - halfText.X, m_trainPosition.Y + nextOffsetY + (tex.Height / 2f) - halfText.Y),
                Color.White,
                0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                1f
            );
        }

        if ((m_trainVelocity.X > 0) && (nextOffsetX > ViewportBounds.Value.Right))
            TryStop();
        else if ((m_trainVelocity.X < 0) && ((nextOffsetX + tex.Width) < ViewportBounds.Value.Left))
            TryStop();
    }
}