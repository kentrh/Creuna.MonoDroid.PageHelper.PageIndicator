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

#### C#

Get the view from markup

```
using Creuna.MonoDroid.PageHelper;

public PageIndicator vwPageIndicator
        {
            get { return Get<PageIndicator>(Resource.Id.pageIndicator); }
        }
```

Set the properties accordingly

```
	var pageIndicator = vwPageIndicator;
	pageIndicator.DotCount = _numberOfPages;
	pageIndicator.ActiveDot = _currentPage;

```

For full documentation on the GrennDroid project see [http://greendroid.cyrilmottier.com/reference/greendroid/widget/PageIndicator.html](http://greendroid.cyrilmottier.com/reference/greendroid/widget/PageIndicator.html).