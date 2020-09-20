using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridBuilder : MonoBehaviour
{
    public Vector2 getHexSkewedPosition( int i, int hx, int hy ) {
		int[] h = { 1, 1, 0, -1, -1, 0, 1, 1, 0 };
		if ( i == 0 ) {
			return Vector2.zero;
		}

		int layer = (int) Mathf.Round( Mathf.Sqrt( (float)i / 3.0f ) );

		int firstIdxInLayer = 3 * layer * (layer-1) + 1;
		int side = (int) (i - firstIdxInLayer) / layer;
		int idx  = (int) (i - firstIdxInLayer) % layer;

		hx = layer*h[side+0] + (idx+1) * h[side+2];
		hy = layer*h[side+1] + (idx+1) * h[side+3];
		return new Vector2(hx, hy);
	}

	public Vector2 getHexPosition( int i, float hx, float hy ) {
		int x = 0;
		int y = 0;
		Vector2 vector = getHexSkewedPosition( i, x, y );
		hx = vector.x - vector.y * 0.5f;
		hy = vector.y * (float)Mathf.Sqrt( 0.75f );
		return new Vector2(hx, hy);
	}
}
