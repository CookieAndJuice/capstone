# Create an engine to run a model

To run a model, create a worker. A worker is the engine that breaks the model down into executable tasks and schedules the tasks to run on a back end (typically the GPU or CPU).

A worker is an instance of an [`IWorker`](xref:Unity.Sentis.IWorker) object.

## Create a Worker

Use [`WorkerFactory.CreateWorker`](xref:Unity.Sentis.ModelAssetExtensions.CreateWorker(Unity.Sentis.ModelAsset,Unity.Sentis.DeviceType,System.Boolean)) to create a worker. You must specify a back end type, which tells Sentis where to run the worker, and a [runtime model](import-a-model-file.md#create-a-runtime-model).

For example, the following code creates a worker that runs on the GPU using Sentis compute shaders.

```
using UnityEngine;
using Unity.Sentis;

public class CreateWorker : MonoBehaviour
{
    ModelAsset modelAsset;
    Model runtimeModel;
    IWorker worker;
    
    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
    }
}
```

## Back end types

Sentis provides CPU and GPU back end types. To understand how Sentis executes operations using the different back ends, refer to [How Sentis runs a model](how-sentis-runs-a-model.md).

If a back end type doesn't support a Sentis layer in a model, the worker will assert. Refer to [Supported ONNX operators](supported-operators.md) for more information.

`BackendType.GPUCompute`, `BackendType.GPUCommandBuffer` and `BackendType.CPU` are the fastest back end types, so use `BackendType.GPUPixel` only if a platform doesn't support compute shaders. To check if your runtime platform supports compute shaders use [SystemInfo.supportsComputeShaders](https://docs.unity3d.com/2023.2/Documentation/ScriptReference/SystemInfo-supportsComputeShaders.html)

If you use `BackendType.CPU` with WebGL, Burst compiles to WebAssembly code which might be slow. Refer to [Getting started with WebGL development](https://docs.unity3d.com/Documentation/Manual/webgl-gettingstarted.html) for more information.

How fast a model runs depends on how well a platform supports multithreading for Burst, or how fully it supports compute shaders. You can [profile a model](profile-a-model.md) to understand the performance of a model.

## Additional resources

- [Create a runtime model](import-a-model-file.md#create-a-runtime-model)
- [How Sentis runs a model](how-sentis-runs-a-model.md)
- [Supported ONNX operators](supported-operators.md)
- [Run a model](run-a-model.md)
