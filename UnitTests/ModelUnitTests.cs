﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ModelUnitTests : BaseTest
    {
        private Model.Model _model;
        private static readonly string _pathDll = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _audioPath = _pathDll + @"\audio\";
        private static readonly string _audioAndVideoPath = _pathDll + @"\audio\video\";

        public ModelUnitTests()
        {
            _model = new Model.Model();
            // Give some time to update 
            Thread.Sleep(30000);
        }
        [TestMethod]
        public void EmptyLink()
        {
            _model.DownloadLink = "";
            _model.DownloadButtonClick();
            Assert.IsTrue(_model.StandardOutput == "Empty link");
        }
        [TestMethod]
        public void WhiteSpaceLink()
        {
            _model.DownloadLink = " ";
            _model.DownloadButtonClick();
            Assert.IsTrue(_model.StandardOutput == "Empty link");
        }
        [TestMethod]
        public void YouTubeLinkNotValid()
        {
            _model.DownloadLink = "Not valid link";
            _model.DownloadButtonClick();
            // Give some time to download AudioDownloader.zip
            Thread.Sleep(4000);
            Assert.IsTrue(_model.StandardOutput == "Error. No file downloaded. Updates are needed.");
        }
        [TestMethod]
        public void DownloadInDifferentQuality()
        {
            var qualities = new List<string> { "raw webm", "raw opus", "raw aac", "superb", "best", "better", "optimal", "very good", 
                                               "transparent", "good", "acceptable", "audio book", "worse", "worst" };
            var expectedFileSizes = new List<long> { 3136081, 3089476, 3083046, 39768753, 6469840, 5518168, 4604680, 4178104, 3692512, 3183856, 2800840, 2486776, 2379040, 1923664 };
            _model.DownloadLink = "https://www.youtube.com/watch?v=4KcQ90UbRsg";
            for (int i = 0; i < qualities.Count; i++)
            {
                _model.SelectedQuality = qualities[i];
                _model.DownloadButtonClick();
                Thread.Sleep(1000);
                while (!_model.IsComboBoxEnabled)
                {
                    Thread.Sleep(100);
                }
                var numberOfFiles = NumberOfFilesInDirectory(_audioPath);
                Assert.IsTrue(numberOfFiles == 1, $"Wrong number of files. Expected number of files: 1, actual number of files: {numberOfFiles}");
                var fileName = FileNamesAndPath(_audioPath);
                var actualFileSize = FileSize(fileName[0]);
                var expetedFileSize = expectedFileSizes[i];
                Console.WriteLine($"Actual file size: {actualFileSize}, expected file size: {expetedFileSize}. Difference: {actualFileSize-expetedFileSize}. File name: {fileName[0]}.");
                Assert.IsTrue(actualFileSize == expetedFileSize, $"Not expected file size. Actual file size: {actualFileSize}, expected file size: {expetedFileSize}, difference: {actualFileSize - expetedFileSize}.");
                DeleteFiles(fileName);
            }
        }
        [TestMethod]
        public void DownloadRawFormats()
        {
            var qualities = new List<string> { "251\twebm", "140\tm4a", "250\twebm", "249\twebm" };
            var expectedFileSizes = new List<long> { 3663749, 3083046, 3560180, 3454030 };
            _model.DownloadLink = "https://www.youtube.com/watch?v=4KcQ90UbRsg";
            for (int i = 0; i < qualities.Count; i++)
            {
                _model.SelectedQuality = qualities[i];
                _model.DownloadButtonClick();
                Thread.Sleep(1000);
                while (!_model.IsComboBoxEnabled)
                {
                    Thread.Sleep(100);
                }
                var numberOfFiles = NumberOfFilesInDirectory(_audioPath);
                Assert.IsTrue(numberOfFiles == 1, $"Wrong number of files. Expected number of files: 1, actual number of files: {numberOfFiles}");
                var fileName = FileNamesAndPath(_audioPath);
                var actualFileSize = FileSize(fileName[0]);
                var expetedFileSize = expectedFileSizes[i];
                Console.WriteLine($"Actual file size: {actualFileSize}, expected file size: {expetedFileSize}. Difference: {actualFileSize - expetedFileSize}. File name: {fileName[0]}.");
                Assert.IsTrue(actualFileSize == expetedFileSize, $"Not expected file size. Actual file size: {actualFileSize}, expected file size: {expetedFileSize}, difference: {actualFileSize - expetedFileSize}.");
                DeleteFiles(fileName);
            }
        }
        [TestMethod]
        public void DownloadAudioAndVideo()
        {
            var qualities = new List<string> { "Audio and video" };
            var expectedFileSizes = new List<long> { 70202974 };
            _model.DownloadLink = "https://www.youtube.com/watch?v=4KcQ90UbRsg";
            for (int i = 0; i < qualities.Count; i++)
            {
                _model.SelectedQuality = qualities[i];
                _model.DownloadButtonClick();
                Thread.Sleep(1000);
                while (!_model.IsComboBoxEnabled)
                {
                    Thread.Sleep(100);
                }
                var numberOfFiles = NumberOfFilesInDirectory(_audioAndVideoPath);
                Assert.IsTrue(numberOfFiles == 1, $"Wrong number of files. Expected number of files: 1, actual number of files: {numberOfFiles}");
                var fileName = FileNamesAndPath(_audioAndVideoPath);
                var actualFileSize = FileSize(fileName[0]);
                var expetedFileSize = expectedFileSizes[i];
                Console.WriteLine($"Actual file size: {actualFileSize}, expected file size: {expetedFileSize}. Difference: {actualFileSize - expetedFileSize}. File name: {fileName[0]}.");
                Assert.IsTrue(actualFileSize == expetedFileSize, $"Not expected file size. Actual file size: {actualFileSize}, expected file size: {expetedFileSize}, difference: {actualFileSize - expetedFileSize}.");
                DeleteFiles(fileName);
            }
        }
        [TestMethod]
        public void DownloadPlayListOne()
        {
            _model.DownloadLink = "https://www.youtube.com/playlist?list=PL9tWYRlGyp4GgQu1liXcY9NT1Geg3Nsok";
            _model.SelectedQuality = "raw aac";
            var expectedFileSizes = new List<long> { 5083565, 4034396, 3402184, 3631459, 2875099, 3873389, 3458799, 5609895, 4151596 };
            _model.DownloadButtonClick();
            Thread.Sleep(1000);
            var allowedSizeDifference = 100;
            while (!_model.IsComboBoxEnabled)
            {
                Thread.Sleep(100);
            }
            var numberOfFiles = NumberOfFilesInDirectory(_audioPath);
            Assert.IsTrue(numberOfFiles == expectedFileSizes.Count, $"Wrong number of files. Expected number of files: {expectedFileSizes.Count}, actual number of files: {numberOfFiles}");
            var fileNames = FileNamesAndPath(_audioPath);
            Console.WriteLine($"Allowed size difference: {allowedSizeDifference}.");
            for (int i = 0; i < numberOfFiles; i++)
            {
                var actualFileSize = FileSize(fileNames[i]);
                var expetedFileSize = expectedFileSizes[i];
                Console.WriteLine($"Actual file size: {actualFileSize}, expected file size: {expetedFileSize}. Difference: {actualFileSize - expetedFileSize}. File name: {fileNames[i]}.");
                Assert.IsTrue((expetedFileSize - allowedSizeDifference <= actualFileSize) && (actualFileSize <= expetedFileSize + allowedSizeDifference), $"Not expected file size. Actual file size: {actualFileSize}, expected file size: {expetedFileSize}, difference: {actualFileSize - expetedFileSize}.");
            }
            DeleteFiles(fileNames);
        }
        [TestMethod]
        public void DownloadPlayListCancel()
        {
            _model.DownloadLink = "https://www.youtube.com/playlist?list=PL9tWYRlGyp4GgQu1liXcY9NT1Geg3Nsok";
            _model.SelectedQuality = "raw aac";
            _model.DownloadButtonClick();
            Thread.Sleep(1000);
            while (!_model.IsComboBoxEnabled)
            {
                // give some time to download a couple of files
                Thread.Sleep(10000);
                // simulating cancel click
                _model.DownloadButtonClick();
                Thread.Sleep(10000);
            }
            var numberOfFiles = NumberOfFilesInDirectory(_audioPath);
            Assert.IsTrue(numberOfFiles >= 3 && numberOfFiles <= 7, $"Wrong number of files. Expected number of files between 3 and 7. Actual number of files: {numberOfFiles}");
            var fileNames = FileNamesAndPath(_audioPath);
            DeleteFiles(fileNames);
        }
        [TestMethod]
        public void DownloadPlayListTwo()
        {
            _model.DownloadLink = "https://www.youtube.com/watch?v=Nxs_mpWt2BA&list=PLczZk1L30r_s_9woWc1ZvhUNA2n_wjICI&index=1";
            _model.SelectedQuality = "raw aac";
            var expectedFileSizes = new List<long> { 2915430, 6544910, 4142954, 7978914, 5816029, 3563543, 3527081, 3649456, 4245393, 2883479, 3484737, 2475309, 3455726, 3840186 };
            _model.DownloadButtonClick();
            Thread.Sleep(1000);
            var allowedSizeDifference = 100;
            while (!_model.IsComboBoxEnabled)
            {
                Thread.Sleep(100);
            }
            var numberOfFiles = NumberOfFilesInDirectory(_audioPath);
            Assert.IsTrue(numberOfFiles == expectedFileSizes.Count, $"Wrong number of files. Expected number of files: {expectedFileSizes.Count}, actual number of files: {numberOfFiles}");
            var fileNames = FileNamesAndPath(_audioPath);
            Console.WriteLine($"Allowed size difference: {allowedSizeDifference}.");
            for (int i = 0; i < numberOfFiles; i++)
            {
                var actualFileSize = FileSize(fileNames[i]);
                var expetedFileSize = expectedFileSizes[i];
                Console.WriteLine($"Actual file size: {actualFileSize}, expected file size: {expetedFileSize}. Difference: {actualFileSize - expetedFileSize}. File name: {fileNames[i]}.");
                Assert.IsTrue((expetedFileSize - allowedSizeDifference <= actualFileSize) && (actualFileSize <= expetedFileSize + allowedSizeDifference), $"Not expected file size. Actual file size: {actualFileSize}, expected file size: {expetedFileSize}, difference: {actualFileSize - expetedFileSize}.");
            }
            DeleteFiles(fileNames);
        }
    }
}
