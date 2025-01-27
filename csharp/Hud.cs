﻿using System.Text;
using DontStarve.Hunger;
using DontStarve.Sanity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace DontStarve;

internal static class Hud {
	private static Vector2 barPosition {
		get {
			var sizeUi = new Vector2(Game1.uiViewport.Width, Game1.uiViewport.Height);
			return new Vector2(sizeUi.X - (Game1.showingHealth ? 171 : 116), sizeUi.Y);
		}
	}

	internal static void OnRenderingHud(IModHelper helper, RenderingHudEventArgs e) {
		if (!Context.IsWorldReady || Game1.CurrentEvent != null) return;
		OnRenderingHunger(e);
		OnRenderingSanity(e);
		OnRenderingTooltip(helper, e);
	}

	private static void OnRenderingHunger(RenderingHudEventArgs e) {
		var player = Game1.player;
		var hunger = player.getHunger();
		var maxHunger = player.getMaxHunger();

		e.SpriteBatch.Draw(
			texture: Textures.hungerContainer,
			destinationRectangle: new Rectangle(
				x: (int)barPosition.X - 60,
				y: (int)barPosition.Y - 240,
				width: Textures.hungerContainer.Width * 4,
				height: Textures.hungerContainer.Height * 4
			),
			color: Color.White
		);

		e.SpriteBatch.Draw(
			texture: Textures.sanityFiller,
			position: new Vector2(barPosition.X - 24, barPosition.Y - 25),
			sourceRectangle: new Rectangle(
				x: 0,
				y: 0,
				width: Textures.sanityFiller.Width * 6 * Game1.pixelZoom,
				height: (int)(hunger / maxHunger * 168)
			),
			color: BarsInformation.hungerColor,
			rotation: 3.138997f,
			origin: new Vector2(0.5f, 0.5f),
			scale: 1f,
			effects: SpriteEffects.None,
			layerDepth: 1f
		);

		var mousePosition = new Vector2(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y);
		var checkXGreater = mousePosition.X >= barPosition.X - 60;
		var checkXLess = mousePosition.X <= barPosition.X - 60 + Textures.sanityContainer.Width * 4;
		var checkYGreater = mousePosition.Y >= barPosition.Y - 240;
		var checkYLess = mousePosition.Y <= barPosition.Y - 240 + Textures.sanityContainer.Height * 4;
		var checkX = checkXGreater && checkXLess;
		var checkY = checkYGreater && checkYLess;

		if (checkX && checkY) {
			var information = $"{Math.Round(hunger)}/{Math.Round(maxHunger)}";
			var textSize = Game1.dialogueFont.MeasureString(information);
			var textPosition = new Vector2(-12, textSize.X);

			Game1.spriteBatch.DrawString(
				spriteFont: Game1.dialogueFont,
				text: information,
				position: new Vector2(
					x: barPosition.X - 60 + textPosition.X,
					y: barPosition.Y - 240 + Textures.sanityContainer.Height + 8
				),
				color: new Color(255, 255, 255),
				rotation: 0f,
				origin: new Vector2(textPosition.Y, 0),
				scale: 1,
				effects: SpriteEffects.None,
				layerDepth: 0f
			);
		}
	}

	private static void OnRenderingSanity(RenderingHudEventArgs e) {
		var player = Game1.player;
		var sanity = player.getSanity();
		var maxSanity = player.getMaxSanity();

		e.SpriteBatch.Draw(
			texture: Textures.sanityContainer,
			destinationRectangle: new Rectangle(
				x: (int)barPosition.X,
				y: (int)barPosition.Y - 240,
				width: Textures.sanityContainer.Width * 4,
				height: Textures.sanityContainer.Height * 4
			),
			color: Color.White
		);

		e.SpriteBatch.Draw(
			texture: Textures.sanityFiller,
			position: new Vector2(barPosition.X + 36, barPosition.Y - 25),
			sourceRectangle: new Rectangle(
				x: 0,
				y: 0,
				width: Textures.sanityFiller.Width * 6 * Game1.pixelZoom,
				height: (int)(sanity / maxSanity * 168)
			),
			color: BarsInformation.sanityColor,
			rotation: 3.138997f,
			origin: new Vector2(0.5f, 0.5f),
			scale: 1f,
			effects: SpriteEffects.None,
			layerDepth: 1f
		);

		var mousePosition = new Vector2(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y);
		var checkXGreater = mousePosition.X >= barPosition.X;
		var checkXLess = mousePosition.X <= barPosition.X + Textures.sanityContainer.Width * 4;
		var checkYGreater = mousePosition.Y >= barPosition.Y - 240;
		var checkYLess = mousePosition.Y <= barPosition.Y - 240 + Textures.sanityContainer.Height * 4;
		var checkX = checkXGreater && checkXLess;
		var checkY = checkYGreater && checkYLess;

		if (checkX && checkY) {
			var information = $"{Math.Round(sanity)}/{Math.Round(maxSanity)}";
			var textSize = Game1.dialogueFont.MeasureString(information);
			var textPosition = new Vector2(-12, textSize.X);

			Game1.spriteBatch.DrawString(
				spriteFont: Game1.dialogueFont,
				text: information,
				position: new Vector2(
					x: barPosition.X + textPosition.X,
					y: barPosition.Y - 240 + Textures.sanityContainer.Height + 8
				),
				color: new Color(255, 255, 255),
				rotation: 0f,
				origin: new Vector2(textPosition.Y, 0),
				scale: 1,
				effects: SpriteEffects.None,
				layerDepth: 0f
			);
		}
	}

	private static void OnRenderingTooltip(IModHelper helper, RenderingHudEventArgs e) {
		var player = Game1.player;

		var activeObject = player.ActiveObject;
		if (activeObject != null) {
			double? foodHunger = Hunger.EatFood.foodHunger.TryGetValue(activeObject.ItemId, out var hungerSanity)
				? hungerSanity
				: null;
			double? foodSanity = Sanity.EatFood.foodSanity.TryGetValue(activeObject.ItemId, out var sanityValue)
				? sanityValue
				: null;
			if (foodSanity != null || foodHunger != null) {
				var sizeUi = new Vector2(Game1.uiViewport.Width, Game1.uiViewport.Height);
				var text = new StringBuilder();
				if (foodHunger != null) {
					text.Append(helper.Translation.Get("hunger-tooltip", new { value = foodHunger }));
				}

				if (foodHunger != null && foodSanity != null) {
					text.AppendLine();
				}

				if (foodSanity != null) {
					text.Append(helper.Translation.Get("sanity-tooltip", new { value = foodSanity }));
				}

				var textSize = Game1.smallFont.MeasureString(text);
				var spriteBatch = e.SpriteBatch;
				IClickableMenu.drawTextureBox(
					b: spriteBatch,
					texture: Game1.menuTexture,
					sourceRect: new Rectangle(0, 256, 60, 60),
					x: (int)(sizeUi.X / 2) - (int)(textSize.X / 2 + 25),
					y: (int)sizeUi.Y - 125 - (int)(textSize.Y + 25),
					width: (int)(textSize.X + 50),
					height: (int)(textSize.Y + 40),
					color: Color.White * 1,
					scale: 1,
					drawShadow: false,
					draw_layer: 1
				);
				Utility.drawTextWithShadow(
					b: spriteBatch,
					text: text,
					font: Game1.smallFont,
					position: new Vector2(
						x: (int)(sizeUi.X / 2) - (int)(textSize.X / 2 + 25) + 25,
						y: (int)sizeUi.Y - 125 - (int)(textSize.Y + 25) + 20
					),
					color: Game1.textColor
				);
			}
		}
	}
}