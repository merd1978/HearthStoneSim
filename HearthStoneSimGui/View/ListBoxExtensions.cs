using System;
using System.Collections.Specialized;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

/*
This Attached Behavior automatically scrolls the listbox to the bottom when a new item is added.

<ListBox ItemsSource="{Binding LoggingStream}">
    <i:Interaction.Behaviors>
        <behaviors:ScrollOnNewItemBehavior 
           IsActiveScrollOnNewItem="{Binding IfFollowTail, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
    </i:Interaction.Behaviors>
</ListBox>
In your ViewModel, you can bind to boolean IfFollowTail { get; set; } to control whether auto scrolling is active or not.

The Behavior does all the right things:

- If IfFollowTail=false is set in the ViewModel, the ListBox no longer scrolls to the bottom on a new item.
- As soon as IfFollowTail=true is set in the ViewModel, the ListBox instantly scrolls to the bottom, and continues to do so.
- It's fast. It only scrolls after a couple of hundred milliseconds of inactivity. A naive implementation would be extremely slow, as it would scroll on every new item added.
- It works with duplicate ListBox items (a lot of other implementations do not work with duplicates - they scroll to the first item, then stop).
- It's ideal for a logging console that deals with continuous incoming items.
You will need to add Reactive Extensions to your project. https://stackoverflow.com/questions/2006729/how-can-i-have-a-listbox-auto-scroll-when-a-new-item-is-added
*/

namespace HearthStoneSimGui.View
{
    public class ScrollOnNewItemBehavior : Behavior<ListBox>
    {
        private IDisposable _rxScrollIntoView;
        ListBox ListBox => this.AssociatedObject;

        public static readonly DependencyProperty IsActiveScrollOnNewItemProperty = DependencyProperty.Register(
            "IsActiveScrollOnNewItem",
            typeof(bool),
            typeof(ScrollOnNewItemBehavior),
            new PropertyMetadata(true, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Intent: immediately scroll to the bottom if our dependency property changes.
            ScrollOnNewItemBehavior behavior = dependencyObject as ScrollOnNewItemBehavior;
            if (behavior == null)
            {
                return;
            }

            behavior.IsActiveScrollOnNewItemMirror = (bool)dependencyPropertyChangedEventArgs.NewValue;

            if (behavior.IsActiveScrollOnNewItemMirror == false)
            {
                return;
            }

            ListboxScrollToBottom(behavior.ListBox);
        }

        public bool IsActiveScrollOnNewItem
        {
            get => (bool)this.GetValue(IsActiveScrollOnNewItemProperty);
            set => this.SetValue(IsActiveScrollOnNewItemProperty, value);
        }

        public bool IsActiveScrollOnNewItemMirror { get; set; } = true;

        protected override void OnAttached()
        {
            this.AssociatedObject.Loaded += this.OnLoaded;
            this.AssociatedObject.Unloaded += this.OnUnLoaded;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= this.OnLoaded;
            this.AssociatedObject.Unloaded -= this.OnUnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var changed = this.AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (changed == null)
            {
                return;
            }

            // Intent: If we scroll into view on every single item added, it slows down to a crawl.
            this._rxScrollIntoView = changed
                .ToObservable()
                .ObserveOn(new EventLoopScheduler(ts => new Thread(ts) { IsBackground = true }))
                .Where(o => this.IsActiveScrollOnNewItemMirror == true)
                .Where(o => o.NewItems?.Count > 0)
                .Sample(TimeSpan.FromMilliseconds(180))
                .Subscribe(o =>
                {
                    this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ListboxScrollToBottom(this.ListBox);
                    }));
                });
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            this._rxScrollIntoView?.Dispose();
        }

        /// <summary>
        /// Scrolls to the bottom. Unlike other methods, this works even if there are duplicate items in the listbox.
        /// </summary>
        private static void ListboxScrollToBottom(ListBox listBox)
        {
            if (VisualTreeHelper.GetChildrenCount(listBox) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }
    }

    public static class ListBoxEventToObservableExtensions
    {
        /// <summary>Converts CollectionChanged to an observable sequence.</summary>
        public static IObservable<NotifyCollectionChangedEventArgs> ToObservable<T>(this T source)
            where T : INotifyCollectionChanged
        {
            return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h);
        }
    }

}
