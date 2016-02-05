# peacock

## Remarks:

To run this program, please copy the following to files to its working directory:

 *  http://datagov.ic.nhs.uk/T201202ADD%20REXT.CSV 
 *  http://datagov.ic.nhs.uk/T201109PDP%20IEXT.CSV 

Before you can run any queries please create the databases, and for optimal results also the index using the buttons on the UI. This will take some time, and need some RAM and disk space.

The data files are just the input files in binary format padded to fixed witdth rows.

The algorithm was writtend with an SSD in mind, so there are multiple seeks during reading the file. If need be, it can be converted to read sequentially.

Have fun with the program!
