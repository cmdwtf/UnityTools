using System;
using System.Linq;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class TerrainExtensions
	{
		/// <summary>
		/// Sets the given texture as the only layer on the given terrain.
		/// </summary>
		/// <param name="terrainData"><see cref="TerrainData"/> to modify the texture of.</param>
		/// <param name="texture">Texture to be used.</param>
		/// <param name="size">Size of the <see cref="Terrain"/> in meters.</param>
		public static void SetTerrainTexture(this TerrainData terrainData, Texture2D texture, float size)
		{
			// clear current layers.
			terrainData.terrainLayers = Array.Empty<TerrainLayer>();

			terrainData.AddTerrainTexture(texture, size);
		}

		/// <summary>
		/// Adds the given texture as an extra layer to the given terrain.
		/// </summary>
		/// <param name="terrainData"><see cref="TerrainData"/> to modify the texture of.</param>
		/// <param name="texture">Texture to be used.</param>
		/// <param name="size">Size of the <see cref="Terrain"/> in meters.</param>
		/// <remarks>
		///	Via: https://forum.unity.com/threads/terrain-layers-api-can-you-tell-me-the-starting-point.606019/#post-4966541
		/// </remarks>
		public static void AddTerrainTexture(this TerrainData terrainData, Texture2D texture, float size)
		{
			var newTextureLayer = new TerrainLayer
			{
				diffuseTexture = texture,
				tileOffset = Vector2.zero,
				tileSize = Vector2.one * size,
			};

			AddTerrainLayer(terrainData, newTextureLayer);
		}

		/// <summary>
		/// Adds new <see cref="TerrainLayer"/> to the given <see cref="TerrainData"/> object.
		/// </summary>
		/// <param name="terrainData"><see cref="TerrainData"/> to add layer to.</param>
		/// <param name="inputLayer"><see cref="TerrainLayer"/> to add.</param>
		/// <remarks>
		///	Via: https://forum.unity.com/threads/terrain-layers-api-can-you-tell-me-the-starting-point.606019/#post-4966541
		/// </remarks>
		public static void AddTerrainLayer(this TerrainData terrainData, TerrainLayer inputLayer)
		{
			if (inputLayer == null)
			{
				return;
			}

			TerrainLayer[] layers = terrainData.terrainLayers;

			if (layers.Any(t => t == inputLayer))
			{
				return;
			}

			int newIndex = layers.Length;
			var newTerrainLayerArray = new TerrainLayer[newIndex + 1];
			Array.Copy(layers, 0, newTerrainLayerArray, 0, newIndex);
			newTerrainLayerArray[newIndex] = inputLayer;

			terrainData.terrainLayers = newTerrainLayerArray;
		}
	}
}
