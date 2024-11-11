Shader "Tutorial/Target" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "Queue" = "Overlay" }
        ColorMask 0
        ZWrite Off
        Stencil {
            Ref 1
            Comp always
            Pass replace
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata_t {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 vertex : SV_POSITION;
            };
            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target {
                return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}
