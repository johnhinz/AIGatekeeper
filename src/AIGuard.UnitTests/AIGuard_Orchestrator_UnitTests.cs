using AIGuard.Broker;
using AIGuard.IRepository;
using AIGuard.Orchestrator;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MQTTnet.Client.Publishing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIGuard.UnitTests
{
    [TestClass]
    public class AIGuard_Orchestrator_UnitTests
    {
        private Mock<IPublishDetections<List<int>>> _detectionPublisher;
        private Worker _worker;

        [TestInitialize]
        public void Init()
        {
            Mock<ILogger<Worker>> logger = new Mock<ILogger<Worker>>();
            Mock<IDetectObjects> detector = new Mock<IDetectObjects>();
            Mock<IPublishDetections<MqttClientPublishResult>> publisher = new Mock<IPublishDetections<MqttClientPublishResult>>();
            List<Camera> cameras = new List<Camera>();
            cameras.Add(
                new Camera()
                {
                    Clip = true,
                    Name = "camera",
                    Watches = new List<Item>() { new Item() { Confidence = 0.5F, Label = "person" } }
                });

            _worker = new Worker(
                logger.Object,
                detector.Object,
                publisher.Object,
                cameras,
                "c:\\temp",
                "*.jpg",
                false
                );
        }

        [TestMethod]
        public void Camera_Find_Positive_Match()
        {
            FileSystemEventArgs e = new FileSystemEventArgs(WatcherChangeTypes.Created, "c:\\temp", "camera");
            Camera result = _worker.FindCamera(e);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Camera_Find_Negative_Match()
        {
            FileSystemEventArgs e = new FileSystemEventArgs(WatcherChangeTypes.Created, "c:\\temp", "microphone");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _worker.FindCamera(e));
        }

        [TestMethod]
        public void DetectTarget_Find_Positive_Match()
        {
            Camera camera = new Camera() {
                Clip = true,
                Name = "test",
                Watches = new List<Item>() { new Item() { Confidence = 0.5F, Label = "person" } } };

            Mock<IDetectedObject> detectedObject = new Mock<IDetectedObject>();
            detectedObject.Setup(d => d.Label).Returns("person");
            detectedObject.Setup(d => d.Confidence).Returns(.5F);

            Mock<List<IDetectedObject>> detectedObjects = new Mock<List<IDetectedObject>>();
            detectedObjects.Object.Add(detectedObject.Object);
            var result = Worker.DetectTarget(camera, new[] { detectedObject.Object });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DetectTarget_Find_LowConfidence_Negative_Match()
        {
            Camera camera = new Camera()
            {
                Clip = true,
                Name = "test",
                Watches = new List<Item>() { new Item() { Confidence = 0.6F, Label = "person" } }
            };

            Mock<IDetectedObject> detectedObject = new Mock<IDetectedObject>();
            detectedObject.Setup(d => d.Label).Returns("person");
            detectedObject.Setup(d => d.Confidence).Returns(.5F);

            Mock<List<IDetectedObject>> detectedObjects = new Mock<List<IDetectedObject>>();
            detectedObjects.Object.Add(detectedObject.Object);
            var result = Worker.DetectTarget(camera, new[] { detectedObject.Object });

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DetectTarget_Find_Negative_Match()
        {
            Camera camera = new Camera()
            {
                Clip = true,
                Name = "test",
                Watches = new List<Item>() { new Item() { Confidence = 0.5F, Label = "car" } }
            };

            Mock<IDetectedObject> detectedObject = new Mock<IDetectedObject>();
            detectedObject.Setup(d => d.Label).Returns("person");
            detectedObject.Setup(d => d.Confidence).Returns(.5F);

            Mock<List<IDetectedObject>> detectedObjects = new Mock<List<IDetectedObject>>();
            detectedObjects.Object.Add(detectedObject.Object);
            var result = Worker.DetectTarget(camera, new[] { detectedObject.Object });

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DetectTarget_NullParam_Camera()
        {
            Mock<IDetectedObject> detectedObject = new Mock<IDetectedObject>();
            detectedObject.Setup(d => d.Label).Returns("person");
            detectedObject.Setup(d => d.Confidence).Returns(.5F);

            Mock<List<IDetectedObject>> detectedObjects = new Mock<List<IDetectedObject>>();
            detectedObjects.Object.Add(detectedObject.Object);

            ArgumentNullException anull = Assert.ThrowsException<ArgumentNullException>(() => Worker.DetectTarget(null, new[] { detectedObject.Object }));
            Assert.IsTrue(anull.Message.Contains("camera"));
        }
        [TestMethod]
        public void DetectTarget_NullParam_DetectedItems()
        {
            Camera camera = new Camera()
            {
                Clip = true,
                Name = "test",
                Watches = new List<Item>() { new Item() { Confidence = 0.5F, Label = "car" } }
            };

            try
            {
                Assert.ThrowsException<ArgumentNullException>(() => Worker.DetectTarget(camera, null));
            }
            catch (ArgumentNullException anull)
            {
                Assert.IsTrue(anull.Message.Contains("detectedItems"));
            }
        }
        [TestMethod]
        public void DetectTarget_NullParam_DetectedItems_Camera()
        {

            try
            {
                Assert.ThrowsException<ArgumentNullException>(() => Worker.DetectTarget(null, null));
            }
            catch (ArgumentNullException anull)
            {
                Assert.IsTrue(anull.Message.Contains("camera"));
                Assert.IsTrue(anull.Message.Contains("detectedItems"));
            }
        }

    }
}
