using FreeTypeSharp;

namespace FontStashSharp.Rasterizers.FreeType
{
	public class FreeTypeLoader
	{
		internal static FreeTypeSource Load(byte[] data, FT_Render_Mode_ renderMode)
		{
			return new FreeTypeSource(data, renderMode);
		}
	}
}
