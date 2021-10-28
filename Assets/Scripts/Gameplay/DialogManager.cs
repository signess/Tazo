using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private int lettersPerSecond;

    private Dialog dialog;
    private int currentLine = 0;
    private bool isTyping;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if(currentLine < dialog.Lines.Count)
            {
                TypeDialog(dialog.Lines[currentLine]).GetAwaiter();

            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    public async Task ShowDialog(Dialog dialog)
    {
        await Task.Yield();

        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        TypeDialog(dialog.Lines[0]).GetAwaiter();

    }

    public async Task TypeDialog(string dialog)
    {
        isTyping = true;
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            await Task.Delay(1000 / lettersPerSecond);
        }
        isTyping = false;
    }
}
