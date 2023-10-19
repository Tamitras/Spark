﻿using System.Collections.ObjectModel;
using SkiaSharp;
using Spark.Class;
using Spark.ViewModels.Base;

namespace Spark.ViewModels
{
    public enum ProcessingStatus
    {
        Converting,
        Connecting,
        Processing,
        Done
    }

    public class PhotoVM : BaseVM
    {
        public PhotoVM(FileResult photo, string name)
        {
            this.RankedList = new ObservableCollection<NumberWithScore>();
            this.OriginalFile = photo;
            this.Status = ProcessingStatus.Converting;
            this.Name = name;

            this.ParseImageTobyteArray();

            this.GetImageDimensions();
        }

        private void GetImageDimensions()
        {
            try
            {
                var inputMemoryStream = new MemoryStream(this.ByteArrayOriginal);
                this.Bitmap = SKBitmap.Decode(inputMemoryStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while parsing ByteArray to Bitmap");
            }
        }

        private async void ParseImageTobyteArray()
        {
            try
            {
                var photoStream = await this.OriginalFile.OpenReadAsync();
                var memoryStream = new MemoryStream();
                photoStream.CopyTo(memoryStream);
                this.ByteArrayOriginal = memoryStream.ToArray();


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while parsing Image to byteArray");
            }
        }

        public string FormattedProcessingTime
        {
            get
            {
                return this.ProcessingTime.ToString(@"ss\:fff");
            }
        }


        public string SizeOriginal
        {
            get
            {
                try
                {
                    return (this.ByteArrayOriginal.Length / (1024.0 *1024)).ToString("F2");
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as necessary
                    return "Error";
                }
            }
        }

        public string SizeThumbnail
        {
            get
            {
                try
                {
                    return (this.ByteArrayThumbnail.Length / (1024.0 * 1024)).ToString("F2");
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as necessary
                    return "Error";
                }
            }
        }


        private string _firstResult;
        public string FirstResult
        {
            get => _firstResult;
            set
            {
                _firstResult = value;
                OnPropertyChanged(nameof(FirstResult));
            }
        }

        private ObservableCollection<NumberWithScore> _rankedList;
        public ObservableCollection<NumberWithScore> RankedList
        {
            get => _rankedList;
            set
            {
                _rankedList = value;
                OnPropertyChanged(nameof(RankedList));
            }
        }

        private ProcessingStatus _status;
        public ProcessingStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));

                // Aktualisieren Sie den StatusString, wenn der Status sich ändert
                StatusString = _status.ToString();
            }
        }

        private string _statusString;
        public string StatusString
        {
            get => _statusString;
            set
            {
                _statusString = value;
                OnPropertyChanged(nameof(StatusString));
            }
        }

        private SKBitmap _bitmap;
        public SKBitmap Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                OnPropertyChanged(nameof(Bitmap));
            }
        }

        private TimeSpan _processingTime;
        public TimeSpan ProcessingTime
        {
            get => _processingTime;
            set
            {
                _processingTime = value;
                OnPropertyChanged(nameof(ProcessingTime));
                OnPropertyChanged(nameof(FormattedProcessingTime));  // Benachrichtigt die UI, dass sich auch FormattedProcessingTime geändert hat
            }
        }


        private FileResult _originalFile;
        public FileResult OriginalFile
        {
            get => _originalFile;
            set
            {
                _originalFile = value;
                OnPropertyChanged(nameof(OriginalFile));
            }
        }

        private byte[] _byteArray;
        public byte[] ByteArrayOriginal
        {
            get => _byteArray;
            set
            {
                _byteArray = value;
                OnPropertyChanged(nameof(ByteArrayOriginal));
            }
        }

        private byte[] _byteArrayThumbnail;
        public byte[] ByteArrayThumbnail
        {
            get => _byteArrayThumbnail;
            set
            {
                _byteArrayThumbnail = value;
                OnPropertyChanged(nameof(ByteArrayThumbnail));
            }
        }

        private ImageSource _thumbnail;
        public ImageSource Thumbnail
        {
            get => _thumbnail;
            set
            {
                _thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));

                Task.Run(async() =>
                {
                   await UpdateThumbnailByteArrayAsync();
                });

            }
        }

        private async Task UpdateThumbnailByteArrayAsync()
        {
            try
            {
                Stream stream = await ((StreamImageSource)this.Thumbnail).Stream(CancellationToken.None);
                byte[] bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                this.ByteArrayThumbnail = bytes;

                OnPropertyChanged(nameof(this.ByteArrayThumbnail));
                OnPropertyChanged(nameof(this.SizeThumbnail));
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine(ex.Message);
            }
        }
    }
}
