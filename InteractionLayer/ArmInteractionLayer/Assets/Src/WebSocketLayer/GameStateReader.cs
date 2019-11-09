using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WebSocketLayer
{
    public class GameStateReader
    {
        private Dictionary<int, Vector3> cubeDirectory;

        public const string TypeKey = "type";
        public const string GameStateTypeValue = "game_state";
        public const string DataKey = "data";
        public const string IDKey = "id";
        public const string PositionKey = "position";

        public IEnumerable<Vector3> CubePositions
        {
            get
            {
                return cubeDirectory.Values;
            }
        }

        public GameStateReader()
        {
            cubeDirectory = new Dictionary<int, Vector3>();
        }

        public bool TryParseState(string jsonString)
        {
            var o = JObject.Parse(jsonString);

            if (o[TypeKey].ToString() != GameStateTypeValue) return false;

            cubeDirectory.Clear();

            if (o.TryGetValue(DataKey, out JToken token))
            {
                var array = JArray.Parse(token.ToString());
                foreach (JObject item in array)
                {
                    int.TryParse(item.GetValue(IDKey).ToString(), out int index);
                    var blah = item.GetValue(PositionKey).Values<float>().ToArray();
                    var position = new Vector3(blah[0], blah[1], blah[2]);
                    cubeDirectory[index] = position;
                }
                return true;
            }
            else
            {
                Debug.LogError("Error: Cannot find field scene_objects");
                return false;
            }
        }
    }
}