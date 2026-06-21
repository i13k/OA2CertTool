# OA2CertTool
OA2CertTool is a utlity to help install Microsoft OEM certificates on OEM Activation 2.x-enabled Windows systems.
## Quick Start
Download the binary from the *Releases* section and run it on the target computer. Select *Install* to automatically install the certificate or *Save...* to save it for later use. Saved license certificates can be installed with
`slmgr /ilc <certificateFile>.xrm-ms`. After installing the certificate, install an appropriate product key (details below).

## What is OEM Activation 2.x?
OEM Activation 2.x is a technology used by Microsoft to enable Windows activation on the factory floor of OEMs. It works **completely offline**. It requires three elements to be present:
* a `SLIC` table in the computer's BIOS (it contains the name of the OEM, a table ID, the OEM's cryptographic key and digital signature, **and the supported version of OEM Activation** (eg. 2.1));
* an `.xrm-ms` certificate file (this is signed by Microsoft and certifies the validity of the OEM's cryptographic key);
* a valid OEM Activation 2.x product key for your Windows edition; a list of keys is available [here](https://d-fault.nl/keys). **Warning:** This is **not** the key printed on the Certificate of Authenticity sticker.
All three must be present for activation to succeed. **Please note:** OEM Activation 2.x is not bound to a specific edition of Windows. If you have a SLIC table for, say, version 2.1 and the `.xrm-ms` certificate for it, you may activate **any** edition of Windows supporting OEM Activation 2.1 (that is, all editions of Windows 7 and Server 2008 R2). While technically possible, this is, of course, **not legal**, because to activate Windows, you need to have a license for the edition you want to activate. However, if you do have the license for the Windows edition you want to activate (such as the one that came with your computer), this is **perfectly legal**.

**NB:** OEM Activation 2.x product keys are not OEM-specific. You may use any key you want.

## Supported OSes
| OS                      | Ver |
|-------------------------|-----|
| Windows Vista           | 2.0 |
| Windows Server 2008     | 2.0 |
| Windows 7               | 2.1 |
| Windows Server 2008 R2  | 2.1 |
| Windows Server 2012     | 2.2 |
| Windows Server 2012 R2  | 2.3 |
| Windows Server 2016     | 2.4 |
| Windows Server 2019     | 2.5 |
| Windows Server 2022     | 2.6 |
| Windows Server 2025     | 2.7 |

OEM Activation 2.x is not available on client operating systems past Windows 7 (that is, Windows 8, 8.1, 10, and 11). They use a completely different system called OEM Activation 3.0, which connects to Microsoft servers for activation.

## Safety
I consider this to be the safest way to activate Windows and use it whenever it's available. It works offline, **does not require tampering** with any system files and was the official way used by OEMs to activate Windows.

## Missing Certificates
If you have a computer for which no certificate was found, please open an issue in the *Issues* tab. Sometimes, it may be possible to retrieve this certificate and add it to the collection.
