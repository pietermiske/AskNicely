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
<b>Windows Credential prompt:</b>
![Screenshot](https://github.com/pietermiske/AskNicely/blob/master/Screenshots/AskNicely_prompt.png?raw=true)

<b>Returned credentials:</b>
![Screenshot](https://github.com/pietermiske/AskNicely/blob/master/Screenshots/AskNicely_returned_creds.png?raw=true)
