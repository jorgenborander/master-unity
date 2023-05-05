using System;
using System.Linq;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Perception.Randomization.Randomizers
{
    /// <summary>
    /// Randomizes the material of objects tagged with a <see cref="MaterialSwapperRandomizerTag"/>
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Material Swap")]
    public class MaterialSwapperRandomizer : Randomizer
    {
        /// <summary>
        /// At the start of each iteration, call the Randomize function on
        /// all found <see cref="MaterialSwapperRandomizerTag"/>'s.
        /// </summary>
        int countFrame;
        protected override void OnIterationStart()
        {
            // Randomize the target material
            countFrame = 0;
        }
        protected override void OnUpdate(){
        if(countFrame == 2){
            // Randomize the target material
            tagManager.Query<MaterialSwapperRandomizerTag>()
                .ToList().ForEach(tag => tag.Randomize());
            }
        countFrame += 1;
        }
    }
}
