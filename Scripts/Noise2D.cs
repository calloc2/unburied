using Godot;

namespace Unburied.Scripts
{
	public partial class Noise2D : TextureRect
	{
		private float timer = 0f;
		private NoiseTexture2D noiseTexture;
		private FastNoiseLite noise;

		public override void _Ready()
		{
			noiseTexture = Texture as NoiseTexture2D;
			if (noiseTexture != null)
			{
				noise = noiseTexture.Noise as FastNoiseLite;
			}
		}

		public override void _Process(double delta)
		{
			timer += (float)delta;
			if (timer > 0.05f)
			{
				if (noise != null)
				{
					noise.Seed = (int)GD.Randi();
				}
				timer = 0f;
			}
		}
	}
}
