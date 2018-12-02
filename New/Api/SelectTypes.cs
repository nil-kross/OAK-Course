﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api {
    public enum SelectTypes {
        Everything = -3,
        Locations = -2,
        Unsupported = -1,
        Nothing = 0,
        Edges = 1,
        Faces = 2,
        /*
        swSelVERTICES = 3,
        swSelDATUMPLANES = 4,
        swSelDATUMAXES = 5,
        swSelDATUMPOINTS = 6,
        swSelOLEITEMS = 7,
        swSelATTRIBUTES = 8,
        swSelSKETCHES = 9,
        swSelSKETCHSEGS = 10,
        swSelSKETCHPOINTS = 11,
        swSelDRAWINGVIEWS = 12,
        swSelGTOLS = 13,
        swSelDIMENSIONS = 14,
        swSelNOTES = 15,
        swSelSECTIONLINES = 16,
        swSelDETAILCIRCLES = 17,
        swSelSECTIONTEXT = 18,
        swSelSHEETS = 19,
        swSelCOMPONENTS = 20,
        swSelMATES = 21,
        swSelBODYFEATURES = 22,
        swSelREFCURVES = 23,
        swSelEXTSKETCHSEGS = 24,
        swSelEXTSKETCHPOINTS = 25,
        swSelHELIX = 26,
        swSelREFERENCECURVES = 26,
        swSelREFSURFACES = 27,
        swSelCENTERMARKS = 28,
        swSelINCONTEXTFEAT = 29,
        swSelMATEGROUP = 30,
        swSelBREAKLINES = 31,
        swSelINCONTEXTFEATS = 32,
        swSelMATEGROUPS = 33,
        swSelSKETCHTEXT = 34,
        swSelSFSYMBOLS = 35,
        swSelDATUMTAGS = 36,
        swSelCOMPPATTERN = 37,
        swSelWELDS = 38,
        swSelCTHREADS = 39,
        swSelDTMTARGS = 40,
        swSelPOINTREFS = 41,
        swSelDCABINETS = 42,
        swSelEXPLVIEWS = 43,
        swSelEXPLSTEPS = 44,
        swSelEXPLLINES = 45,
        swSelSILHOUETTES = 46,
        swSelCONFIGURATIONS = 47,
        swSelOBJHANDLES = 48,
        swSelARROWS = 49,
        swSelZONES = 50,
        swSelREFEDGES = 51,
        swSelREFFACES = 52,
        swSelREFSILHOUETTE = 53,
        swSelBOMS = 54,
        swSelEQNFOLDER = 55,
        swSelSKETCHHATCH = 56,
        swSelIMPORTFOLDER = 57,
        swSelVIEWERHYPERLINK = 58,
        swSelMIDPOINTS = 59,
        swSelCUSTOMSYMBOLS = 60,
        swSelCOORDSYS = 61,
        swSelDATUMLINES = 62,
        swSelROUTECURVES = 63,
        swSelBOMTEMPS = 64,
        swSelROUTEPOINTS = 65,
        swSelCONNECTIONPOINTS = 66,
        swSelROUTESWEEPS = 67,
        swSelPOSGROUP = 68,
        swSelBROWSERITEM = 69,
        swSelFABRICATEDROUTE = 70,
        swSelSKETCHPOINTFEAT = 71,
        swSelEMPTYSPACE = 72,
        swSelCOMPSDONTOVERRIDE = 72,
        swSelLIGHTS = 73,
        swSelWIREBODIES = 74,
        swSelSURFACEBODIES = 75,
        swSelSOLIDBODIES = 76,
        swSelFRAMEPOINT = 77,
        swSelSURFBODIESFIRST = 78,
        swSelMANIPULATORS = 79,
        swSelPICTUREBODIES = 80,
        swSelSOLIDBODIESFIRST = 81,
        swSelLEADERS = 84,
        swSelSKETCHBITMAP = 85,
        swSelDOWELSYMS = 86,
        swSelEXTSKETCHTEXT = 88,
        swSelBLOCKINST = 93,
        swSelFTRFOLDER = 94,
        swSelSKETCHREGION = 95,
        swSelSKETCHCONTOUR = 96,
        swSelBOMFEATURES = 97,
        swSelANNOTATIONTABLES = 98,
        swSelBLOCKDEF = 99,
        swSelCENTERMARKSYMS = 100,
        swSelSIMULATION = 101,
        swSelSIMELEMENT = 102,
        swSelCENTERLINES = 103,
        swSelHOLETABLEFEATS = 104,
        swSelHOLETABLEAXES = 105,
        swSelWELDMENT = 106,
        swSelSUBWELDFOLDER = 107,
        swSelEXCLUDEMANIPULATORS = 111,
        swSelREVISIONTABLE = 113,
        swSelSUBSKETCHINST = 114,
        swSelWELDMENTTABLEFEATS = 116,
        swSelBODYFOLDER = 118,
        swSelREVISIONTABLEFEAT = 119,
        swSelSUBATOMFOLDER = 121,
        swSelWELDBEADS = 122,
        swSelEMBEDLINKDOC = 123,
        swSelJOURNAL = 124,
        swSelDOCSFOLDER = 125,
        swSelCOMMENTSFOLDER = 126,
        swSelCOMMENT = 127,
        swSelSWIFTANNOTATIONS = 130,
        swSelSWIFTFEATURES = 132,
        swSelCAMERAS = 136,
        swSelMATESUPPLEMENT = 138,
        swSelANNOTATIONVIEW = 139,
        swSelGENERALTABLEFEAT = 142,
        swSelDISPLAYSTATE = 148,
        swSelSUBSKETCHDEF = 154,
        swSelSWIFTSCHEMA = 159,
        swSelTITLEBLOCK = 192,
        swSelTITLEBLOCKTABLEFEAT = 206,
        swSelOBJGROUP = 207,
        swSelPLANESECTIONS = 219,
        swSelCOSMETICWELDS = 220,
        SwSelMAGNETICLINES = 225,
        swSelREVISIONCLOUDS = 240
        */
    }
}