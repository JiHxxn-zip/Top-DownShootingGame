using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // CustomEditor 키워드로 이 에디터 스크립트가 다룰것이라 선언한 오브젝트는 Target으로 접근할 수 있게 자동으로 설정됨
        MapGenerator map = target as MapGenerator;

        map.GenerateMap();
    }
}
