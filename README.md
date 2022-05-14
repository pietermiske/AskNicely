# AskNicely
This C# based executable starts a customizable Windows authentication prompt and can optionally verify if the submitted credentials are valid in the context of the domain or local system. 


### Usage:
```

 █████  ███████ ██   ██ ███    ██ ██  ██████ ███████ ██      ██    ██
██   ██ ██      ██  ██  ████   ██ ██ ██      ██      ██       ██  ██
███████ ███████ █████   ██ ██  ██ ██ ██      █████   ██        ████
██   ██      ██ ██  ██  ██  ██ ██ ██ ██      ██      ██         ██
██   ██ ███████ ██   ██ ██   ████ ██  ██████ ███████ ███████    ██


Optional arguments:                     Discription:
-------------------                     ------------
/help                                   This help menu
/verify                                 Validates submitted credentials in the context of the local system or domain
/title:<title>                          Custom title of the credential prompt
/message:<whatever message>             Custom message shown in the credential prompt


Usage examples:
---------------
Simple credential prompt with default Outlook text: AskNicely.exe
Customized prompt with creds verification: AskNicely.exe /verify /title:"Custom title" /message:"Custom message"
```


### Screenshot:
Windows Credential prompt:
![Screenshot](https://github.com/pietermiske/AskNicely/blob/main/Screenshots/AskNicely_prompt.png?raw=true)

Returned credentials:
![Screenshot](https://github.com/pietermiske/AskNicely/blob/main/Screenshots/AskNicely_returned_creds.png?raw=true)
