Creuna.MonoDroid.PageHelper.PageIndicator
=========================================

Xamarin Android widget for an easy page indicator ported from GreenDroid


#### AXML
```
<creuna.monodroid.pagehelper.PageIndicator
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/pageIndicator"
        android:layout_gravity="bottom"
        android:layout_marginBottom="5dip" />
```

#### CSharp

Get the view from markup

```
using Creuna.MonoDroid.PageHelper;

var pageIndicator = Activity.FindViewById<PageIndicator>(Resource.Id.pageIndicator);
```

Set the properties accordingly

```
	pageIndicator.DotCount = _numberOfPages;
	pageIndicator.ActiveDot = _currentPage;

```

For full documentation on the GrennDroid project see [http://greendroid.cyrilmottier.com/reference/greendroid/widget/PageIndicator.html](http://greendroid.cyrilmottier.com/reference/greendroid/widget/PageIndicator.html).