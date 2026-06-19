using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshBakerEditor : Editor
{
    [MenuItem("Tools/Bake Selected Skinned Mesh")]
    public static void BakeMesh()
    {
        // Проверяем, выбрано ли что-то в иерархии
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogError("Ничего не выбрано! Выделите объект со SkinnedMeshRenderer.");
            return;
        }

        SkinnedMeshRenderer skinnedMeshRenderer = selectedObject.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("На выбранном объекте (или его детях) не найден SkinnedMeshRenderer.");
            return;
        }

        // Создаем новый экземпляр меша
        Mesh bakedMesh = new Mesh();

        // Главная магия: "запекаем" текущую позу в меш
        skinnedMeshRenderer.BakeMesh(bakedMesh, true);

        // Убеждаемся, что папка для экспорта существует
        string folderPath = "Assets/Models/BakedMeshes";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Генерируем уникальное имя файла
        string fileName = $"{selectedObject.name}_Baked_{System.DateTime.Now:HHmmss}.asset";
        string fullPath = System.IO.Path.Combine(folderPath, fileName);

        // Сохраняем меш как ассет в проекте
        AssetDatabase.CreateAsset(bakedMesh, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>Меш успешно запечен и сохранен в: {fullPath}</color>");

        // Опционально: выделяем созданный файл в окне Project
        EditorGUIUtility.PingObject(bakedMesh);
    }

    // Проверка: кнопка в меню будет активна только если выбран объект
    [MenuItem("Tools/Bake Selected Skinned Mesh", true)]
    private static bool ValidateBakeMesh()
    {
        return Selection.activeGameObject != null;
    }
}