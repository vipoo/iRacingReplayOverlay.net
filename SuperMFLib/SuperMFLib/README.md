#SuperMFLib

SuperMFLib is an adapter/wrapper for the MediaFoundation dot.net library developed by [snarfle](http://sourceforge.net/u/snarfle/profile/)
Thanks goes to snarfle 

The goal of the project it to further enhance access to the Media Foundation libraries, using more conventional dot.net interfaces and objects.

So instead of:

    hr = MFExtern.MFStartup(0x10070, MFStartup.Full);
    MFError.ThrowExceptionForHR(hr);
    
       ...
    
    hr = MFExtern.MFShutdown();
    MFError.ThrowExceptionForHR(hr);
    
We can have

    using (MFSystem.Start())
    {
        ...
    }
    
###Known Issue
Due to the difference in memory/object life-cycles of native and managed code, there will be issue on effective memory/object collection. 

I have only focused on performing transcoding of video files at this stage. Will it be possible to encapsulate the library for other things (such as playing video files, etc)?  I dont know!

###Documentation

This is it - checkout the samples.  Further work needs to be done to convert various samples, of both the snarfle library and Microsoft c++ samples.

###Another Example

I developed this library to help me transcode video stream.  As such, the one and only sample in this project, demonstrates how to transcode a source video file to a new file .

The key bits of the sample are here:

    using (MFSystem.Start())
    {
        var readWriteFactory = new ReadWriteClassFactory();

        var attributes = new Attributes
        {
            ReadWriterEnableHardwareTransforms = true,
            SourceReaderEnableVideoProcessing = true,
        };

        var destAttributes = new Attributes
        {
            ReadWriterEnableHardwareTransforms = true,
            SourceReaderEnableVideoProcessing = true,
            MaxKeyFrameSpacing = 3000
        };

        var sourceReader = readWriteFactory.CreateSourceReaderFromURL(sourceFile, attributes);
        var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(destinationFile, destAttributes);

        var writeToSink = ConnectStreams(sourceReader, sinkWriter);

        using (sinkWriter.BeginWriting())
        {
            sourceReader.Samples(sample =>
                {
                    // You can do other things to the sample here
                    //eg cut bits, overlay graphics, etc.
                    return writeToSink(sample);
                });
    }

