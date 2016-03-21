using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace hw1
{
    public sealed partial class MainPage : Page {

        private delegate string AnimalSpeak(object sender, CountEvent counter);
        private event AnimalSpeak speak;
        private int times = 0;

        interface Animal {
            string speak(object sender, CountEvent counter);
        }

        class Cat : Animal {
            public string speak(object sender, CountEvent counter) {
                return "#" + counter.count + " cat : I am a cat\n";
            }
        }

        class Dog : Animal {
            public string speak(object sender, CountEvent counter) {
                return "#" + counter.count + " dog : I am a dog\n";
            }
        }

        class Pig : Animal {
            public string speak(object sender, CountEvent counter) {
                return "#" + counter.count + " pig : I am a pig\n";
            }
        }

        static Cat c = new Cat();
        static Dog d = new Dog();
        static Pig p = new Pig();

        Animal[] animals = { c, d, p };
        Dictionary<string, Animal> animalDictionary = new Dictionary<string, Animal>() {
            {"cat", c },
            {"dog", d },
            {"pig", p }
        };

        public MainPage() {
            this.InitializeComponent();
        }

        public void speakBtnClick(object sender, RoutedEventArgs e) {
            Random rnd = new Random();
            int select = rnd.Next(animals.Count());
            speak = new AnimalSpeak(animals[select].speak);
            speakTextBlock.Text += speak(this, new CountEvent(times++));
        }

        public void confirmBtnClick(object sender, RoutedEventArgs e) {
            if (animalDictionary.ContainsKey(nameTextBox.Text)) {
                speak = new AnimalSpeak(animalDictionary[nameTextBox.Text].speak);
                speakTextBlock.Text += speak(this, new CountEvent(times++));
            }
            nameTextBox.Text = "";
        }

        class CountEvent : EventArgs {
            public int count = 0;
            public CountEvent(int count) {
                this.count = count;
            }
        }
    }
}
