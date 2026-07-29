using System.Collections.Generic;
using UnityEngine;

public static class JsonListHelper {

    [System.Serializable]
    private struct Wrapper<T> {
        public List<T> Items;
    }

    public static string ToJson<T>(List<T> list) {
        Wrapper<T> wrapper = new Wrapper<T> { Items = list };
        return JsonUtility.ToJson(wrapper);
    }

    public static List<T> FromJson<T>(string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items ?? new List<T>();
    }
}