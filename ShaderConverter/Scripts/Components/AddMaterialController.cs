using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using StubbedConverter;

namespace StubbedConverter.Components
{
    public class AddMaterialController : MonoBehaviour
    {
        public bool debug;
        public void Start()
        {
            MaterialController.AddMaterialController(this.gameObject, this.debug);
        }
    }
}
