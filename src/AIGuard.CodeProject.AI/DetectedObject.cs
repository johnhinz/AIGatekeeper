using AIGuard.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGuard.CodePorject.AI
{
    /*
     * {
  "success": (Boolean) // True if successful.
  "message": (String) // A summary of the inference operation.
  "error": (String) // (Optional) An description of the error if success was false.
  "predictions": (Object) // An array of objects with the x_max, x_min, max, y_min, label and confidence.
  "count": (Integer) // The number of objects found.
  "command": (String) // The command that was sent as part of this request. Can be detect, list, status.
  "moduleId": (String) // The Id of the module that processed this request.
  "executionProvider": (String) // The name of the device or package handling the inference. eg CPU, GPU, TPU, DirectML.
  "canUseGPU": (Boolean) // True if this module can use the current GPU if one is present.
  "inferenceMs": (Integer) // The time (ms) to perform the AI inference.
  "processMs": (Integer) // The time (ms) to process the image (includes inference and image manipulation operations).
  "analysisRoundTripMs": (Integer) // The time (ms) for the round trip to the analysis module and back.
    }
    */
    internal class DetectedObject : IDetectedObject
    {
        public float Confidence { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Label { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int YMin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int XMin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int YMax { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int XMax { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
