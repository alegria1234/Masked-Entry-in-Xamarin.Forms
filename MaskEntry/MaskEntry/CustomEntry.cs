using System.Collections.Generic;
using Xamarin.Forms;

namespace MaskEntry
{
    public class CustomEntry : Entry
    {
        public int CursorPositionAuxi { get; set; }
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public static readonly BindableProperty MaskProperty = BindableProperty.Create(
                                          propertyName: "Mask",
                                          returnType: typeof(string),
                                          declaringType: typeof(CustomEntry),
                                          defaultValue: string.Empty,
                                          defaultBindingMode: BindingMode.TwoWay,
                                          propertyChanged: MaskPropertyChanged);



        private static void MaskPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomEntry)bindable;
            if (control == null)
            {
                return;
            }
            control.Keyboard = Keyboard.Telephone;
            control.MaxLength = ((string)newValue).Length + 1;
            control.SetPositionsMask();

            control.Text = control.CreateMarking(out int positionStart);
           
            bool newPositionCursor = false;

            control.TextChanged += (o, e) =>
            {
                try
                {
                    var entry = o as CustomEntry;
                    if (string.IsNullOrWhiteSpace(entry.Text) || control.positionsMask == null)
                    {
                        return;
                    }

                    if (newPositionCursor)
                    {
                        entry.CursorPosition = entry.CursorPositionAuxi;
                        newPositionCursor = false;
                        return;
                    }

                    if (e.NewTextValue.Length != 0)
                    {
                        if (e.NewTextValue.Length > e.OldTextValue.Length)
                        {
                            int cursorposition = entry.CursorPosition;

                            if (e.NewTextValue.Length > cursorposition + 1)
                            {
                                string text = e.NewTextValue;
                                string valeu = control.GetValeuPositionMasck(cursorposition);
                                int index = cursorposition;

                                if (valeu != null)
                                {
                                    while (control.GetValeuPositionMasck(index) != null)
                                    {
                                        index++;
                                    }

                                    char ch = e.NewTextValue[cursorposition];
                                    text = e.OldTextValue.Insert(index, ch.ToString());
                                    index++;
                                }
                                else
                                {
                                    index++;
                                }

                                valeu = control.GetValeuPositionMasck(index);
                                text = text.Remove(index, 1);

                                if (valeu != null)
                                {
                                    index++;
                                }
                                entry.Text = text;
                                entry.CursorPositionAuxi = index;
                                newPositionCursor = true;
                                return;
                            }
                        }
                        else if (e.NewTextValue.Length < control.Mask.Length)
                        {
                            int index = entry.CursorPosition - 1;

                            string valeu = control.GetValeuPositionMasck(index);
                            if (valeu != null)
                            {
                                entry.Text = e.NewTextValue.Insert(index, valeu);
                            }
                            else
                            {
                                entry.Text = e.NewTextValue.Insert(index, "_");
                            }
                            entry.CursorPositionAuxi = index;
                            newPositionCursor = true;
                            return;
                        }
                    }

                    if (e.NewTextValue.Length > control.Mask.Length)
                    {
                        entry.Text = e.NewTextValue.Substring(0, control.Mask.Length);
                    }
                }
                catch
                {

                }
            };
        }

        private void SetPositionsMascara()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                positionsMask = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
            {
                if (Mask[i] != '0')
                {
                    list.Add(i, Mask[i]);
                }
            }
            positionsMask = list;
        }

        private string GetValeuPositionMasck(int positionFind)
        {
            if (!positionsMask.ContainsKey(positionFind))
            {
                return null;
            }

            foreach (var position in positionsMask)
            {
                if (position.Key == positionFind)
                {
                    return position.Value.ToString();
                }
            }

            return null;
        }

        private string CreateMarking(out int positionStart)
        {
            string text = "";
            positionStart = 0;

            for (int i = 0; Mask.Length > i; i++)
            {
                text += "_";
            }

            foreach (var position in positionsMask)
            {
                text = text.Insert(position.Key, position.Value.ToString());
                if (position.Key < positionStart)
                {
                    positionStart = position.Key;
                }
            }

            return text.Substring(0, Mask.Length);
        }

        private void SetPositionsMask()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                positionsMask = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
            {
                if (Mask[i] != '0')
                {
                    list.Add(i, Mask[i]);
                }
            }

            positionsMask = list;
        }

        private IDictionary<int, char> positionsMask;
    }
}
