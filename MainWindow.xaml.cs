using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.VisualBasic.FileIO;
using System.Net;
using System.IO;
using Syncfusion.UI.Xaml.Charts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Shapes;
using System.Diagnostics;

namespace CoronaData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        ViewStateData myData = new ViewStateData();
        ViewStateData myData2 = new ViewStateData();
        ViewStateData myData30 = new ViewStateData();
        ViewStateData myData7 = new ViewStateData();
        ViewStateData myDataCumulative = new ViewStateData();

        public MainWindow()
        {
            //get data file from new york times repository and write it to local file CoronaData.txt
            InitializeComponent();
            string html = string.Empty;
            string StateData = string.Empty;
            string RollingDeathTotal = string.Empty;
            string StateDataUrl = @"https://raw.githubusercontent.com/nytimes/covid-19-data/master/live/us-states.csv";
            string UnitedStatesDataUrl = @"https://raw.githubusercontent.com/nytimes/covid-19-data/master/live/us.csv";
            string UnitedStatesDailyDeathsUrl = @"https://raw.githubusercontent.com/nytimes/covid-19-data/master/rolling-averages/us.csv";
            string UnitedStatesDailyPath = "UnitedStatesDaily.txt";
            string StateDataPath = "StateCoronaData.txt";
            string UnitedStatesDailyDeathsPath = "RollingDeathTotal.txt";

            GetData(UnitedStatesDataUrl, UnitedStatesDailyPath);
            GetData(StateDataUrl, StateDataPath);
            GetData(UnitedStatesDailyDeathsUrl, UnitedStatesDailyDeathsPath);
            
            //initialize data structure and fill it by reading file for state data
            using (StreamReader reader = new System.IO.StreamReader(@"D:\\PICTURES\\StateCoronaData.txt"))
            {
                reader.ReadLine(); // skip first

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] result = line.Split(',');
                    if(result[1] == "American Samoa" || result[1] == "Guam" || result[1] == "Northern Mariana Islands" || result[1] == "Virgin Islands")
                    {
                        line = reader.ReadLine();
                    }
                    result = line.Split(',');
                    myData.Data.Add(new State(result[1], result[4] ,result[3]));
                }
            }
            //adding state populations and calculating per capita deaths
            using (StreamReader reader = new System.IO.StreamReader(@"D:\\PICTURES\\StatePopulations.txt"))
            {
                reader.ReadLine(); // skip first
                int i = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] result = line.Split(',');
                    if(result[2] == "r")
                    {
                        myData.Data[i].Color = Brushes.Red;
                    }
                    if(result[2] == "d")
                    {
                        myData.Data[i].Color = Brushes.Blue;
                    }
                    myData.Data[i].Population = int.Parse(result[1]);
                    myData.Data[i].PerCapitaDeaths = Math.Floor((double)myData.Data[i].ConfirmedDeaths / (double)(myData.Data[i].Population / 100000));
                    i++;
                    if (i == 52)
                    {
                        break;
                    }

                }
            }
            
            //initilaize data structure and fill it by reading rolling death data for usa
            using (StreamReader reader = new System.IO.StreamReader(@"D:\\PICTURES\\RollingDeathTotal.txt"))
            {
                reader.ReadLine(); // skip first
                int i = 0;
                int deathTotal = 0;
                int casesTotal = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] result = line.Split(',');
                    myData2.Data.Add(new State(result[1], result[5], result[2]));
                    myData2.Data[i].Date = result[0];
                    myDataCumulative.Data.Add(new State(result[1], result[5], result[2]));
                    myDataCumulative.Data[i].Date = result[0];
                    deathTotal += int.Parse(result[5]);
                    casesTotal += int.Parse(result[2]);
                    myDataCumulative.Data[i].TotalDeaths = deathTotal;
                    myDataCumulative.Data[i].TotalCases = casesTotal;
                    i++;
                }
            }
            //initialize data structure and fill it by reading file for usa data
            using (StreamReader reader = new System.IO.StreamReader(@"D:\\PICTURES\\UnitedStatesDaily.txt"))
            {
                reader.ReadLine(); // skip first

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] result = line.Split(',');
                    ViewUnitedStatesData myUSAData = new ViewUnitedStatesData(result[1], result[2]);
                    Deaths.Content = myUSAData.ConfirmedDeaths.ToString("###,###,###");
                    Cases.Content = myUSAData.ConfirmedCases.ToString("###,###,###");
                    deathsChange.Content = "+" + myData2.Data[myData2.Data.Count - 1].ConfirmedDeaths.ToString("###,###,###");
                    casesChange.Content = "+" + myData2.Data[myData2.Data.Count - 1].ConfirmedCases.ToString("###,###,###");
                }
            }
            //Initializing data structures for both 7 and 30 day graphs
            int thirty = myData2.Data.Count - 30;
            int seven = myData2.Data.Count - 7;

            for (int i = 0; i < 30; i++)
            {
                myData30.Data.Add(new State(myData2.Data[thirty + i].Name, myData2.Data[thirty + i].ConfirmedDeaths.ToString(), myData2.Data[thirty + i].ConfirmedCases.ToString()));
                myData30.Data[i].Date = myData2.Data[thirty + i].Date;
            }
            for (int i = 0; i < 7; i++)
            {
                myData7.Data.Add(new State(myData2.Data[seven + i].Name, myData2.Data[seven + i].ConfirmedDeaths.ToString(), myData2.Data[seven + i].ConfirmedCases.ToString()));
                myData7.Data[i].Date = myData2.Data[seven + i].Date;

            }

        }

        private void maximumSortOnClick(object sender, RoutedEventArgs e)
        {
            //removes old chart and replaces it with one built in this button
            SfChart chart = new SfChart()
            {
                Palette = ChartColorPalette.Metro,
                Header = "Covid-19 Deaths by State"
            };

            chart.PrimaryAxis = new CategoryAxis()
            { 
                Header = "State",
                LabelsIntersectAction = AxisLabelsIntersectAction.None
            };

            chart.SecondaryAxis = new NumericalAxis()
            { 
                Header = "Deaths" 
            };

            BarSeries series = new BarSeries()
            {
                ItemsSource = myData.Data,
                Label = "State",
                XBindingPath = "Name",
                YBindingPath = "ConfirmedDeaths",
                IsSortData = true,
                ListenPropertyChange = true,
                SortBy = SortingAxis.Y,
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                SortDirection = Direction.Ascending
            };


            ChartAdornmentInfo adornmentInfo = new ChartAdornmentInfo()
            {
                ShowLabel = true,
                LabelPosition = AdornmentsLabelPosition.Outer,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(1),
                FontStyle = FontStyles.Italic,
                SegmentLabelContent = LabelContent.YValue,
                FontFamily = new FontFamily("Calibri"),
                FontSize = 11
            };

            series.AdornmentsInfo = adornmentInfo;
            ChartSeriesBase.SetSpacing(series, .1);

            //Adding Series to the Chart Series Collection

            if(myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);


        }


        private void percapitaOnClick(object sender, RoutedEventArgs e)
        {
            //removes old chart and replaces it with one built in this button
            SfChart chart = new SfChart()
            {
                Palette = ChartColorPalette.Metro,
                Header = "Covid-19 Per Capita Deaths by State"
            };

            chart.PrimaryAxis = new CategoryAxis() 
            { 
                Header = "State" 
            };
            chart.SecondaryAxis = new NumericalAxis() 
            { 
                Header = "Deaths (Per 100,000)" 
            };

            BarSeries series = new BarSeries()
            {
                ItemsSource = myData.Data,
                Label = "State",
                XBindingPath = "Name",
                YBindingPath = "PerCapitaDeaths",
                IsSortData = true,
                ListenPropertyChange = true,
                SortBy = SortingAxis.Y,
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                SortDirection = Direction.Ascending,
            };

            ChartAdornmentInfo adornmentInfo = new ChartAdornmentInfo()
            {
                ShowLabel = true,
                LabelPosition = AdornmentsLabelPosition.Outer,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(1),
                FontStyle = FontStyles.Italic,
                SegmentLabelContent = LabelContent.YValue,
                FontFamily = new FontFamily("Calibri"),
                FontSize = 11
            };

            series.AdornmentsInfo = adornmentInfo;
            ChartSeriesBase.SetSpacing(series, .1);

            //Adding Series to the Chart Series Collection

            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);
        }

        private void rollingDeathOnClick(object sender, RoutedEventArgs e)
        {
            SfChart chart = new SfChart()
            {
                Palette = ChartColorPalette.Metro,
                Header = "Total Covid-19 Deaths Tally",
                Margin = new Thickness(25, 25, 25, 25)
            };

            chart.PrimaryAxis = new CategoryAxis()
            {
                Header = "Date",
                LabelRotationAngle = -60
            };

            chart.SecondaryAxis = new NumericalAxis()
            {
                Minimum = 0,
                Header = "Total Deaths",
                StartRangeFromZero = true
                
            };

            SplineSeries series = new SplineSeries()
            {
                ItemsSource = myDataCumulative.Data,
                XBindingPath = "Date",
                YBindingPath = "TotalDeaths",
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                ShowTooltip = true
            };

            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);
        }

        private void politicalLeaningOnClick(object sender, RoutedEventArgs e)
        {
            //removes old chart and replaces it with one built in this button
            SfChart chart = new SfChart() 
            {
                Palette = ChartColorPalette.Metro,
                Header = "Covid-19 Per Capita Deaths by State"
            };

            chart.PrimaryAxis = new CategoryAxis() 
            { 
                Header = "State" 
            };
            chart.SecondaryAxis = new NumericalAxis() 
            { 
                Header = "Deaths (Per 100,000)" 
            };

            BarSeries series = new BarSeries()
            {
                ItemsSource = myData.Data,
                Label = "State",
                XBindingPath = "Name",
                YBindingPath = "PerCapitaDeaths",
                SegmentColorPath = "Color",
                IsSortData = true,
                ListenPropertyChange = true,
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
            };

            ChartAdornmentInfo adornmentInfo = new ChartAdornmentInfo()
            {
                ShowLabel = true,
                LabelPosition = AdornmentsLabelPosition.Outer,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(1),
                FontStyle = FontStyles.Italic,
                SegmentLabelContent = LabelContent.YValue,
                FontFamily = new FontFamily("Calibri"),
                FontSize = 11
            };

            series.AdornmentsInfo = adornmentInfo;
            ChartSeriesBase.SetSpacing(series, .1);

            //Adding Series to the Chart Series Collection

            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);
        }

        private void rollingSevenDeathOnClick(object sender, RoutedEventArgs e)
        {
            SfChart chart = new SfChart()
            {
                Margin = new Thickness(25, 25, 25, 25),
                Palette = ChartColorPalette.Metro,
                Header = "Rolling Death Tally Last 7 Days"
            };



            chart.PrimaryAxis = new CategoryAxis()
            {
                Header = "Date",
                PlotOffset = 10,
                LabelRotationAngle = -60
            };


            chart.SecondaryAxis = new NumericalAxis()
            {
                Minimum = 0,
                Header = "Deaths",
                StartRangeFromZero = true

            };

            SplineSeries series = new SplineSeries()
            {
                ItemsSource = myData7.Data,
                XBindingPath = "Date",
                YBindingPath = "ConfirmedDeaths",
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                ShowTooltip = true
            };

            ChartAdornmentInfo adornmentInfo = new ChartAdornmentInfo()
            {
                ShowLabel = true,
                LabelPosition = AdornmentsLabelPosition.Auto,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(1),
                FontStyle = FontStyles.Italic,
                SegmentLabelContent = LabelContent.YValue,
                FontFamily = new FontFamily("Calibri"),
                FontSize = 11
            };
            series.AdornmentsInfo = adornmentInfo;
            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);
        }

        private void rollingThirtyDeathOnClick(object sender, RoutedEventArgs e)
        {
            SfChart chart = new SfChart()
            {
                Margin = new Thickness(25, 25, 25, 25),
                Palette = ChartColorPalette.Metro,
                Header = "Rolling Death Tally Last 30 Days"
            };

            chart.PrimaryAxis = new CategoryAxis()
            {
                Header = "Date",
                PlotOffset = 10,
                LabelRotationAngle = -60
            };

            chart.SecondaryAxis = new NumericalAxis()
            {
                Minimum = 0,
                Header = "Deaths",
                StartRangeFromZero = true

            };

            SplineSeries series = new SplineSeries()
            {
                ItemsSource = myData30.Data,
                XBindingPath = "Date",
                YBindingPath = "ConfirmedDeaths",
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                ShowTooltip = true
            };
            ChartAdornmentInfo adornmentInfo = new ChartAdornmentInfo()
            {
                ShowLabel = true,
                LabelPosition = AdornmentsLabelPosition.Auto,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(1),
                FontStyle = FontStyles.Italic,
                SegmentLabelContent = LabelContent.YValue,
                FontFamily = new FontFamily("Calibri"),
                FontSize = 11
            };

            series.AdornmentsInfo = adornmentInfo;

            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);
        }

        private void rollingCasesOnClick(object sender, RoutedEventArgs e)
        {
            SfChart chart = new SfChart() 
            {
                Palette = ChartColorPalette.Metro,
                Header = "Total Covid-19 Cases Tally",
                Margin = new Thickness(25, 25, 25, 25)
            };



            chart.PrimaryAxis = new CategoryAxis()
            {
                Header = "Date",
                LabelRotationAngle = -60
            };

            chart.SecondaryAxis = new NumericalAxis()
            {
                Minimum = 0,
                Header = "Total Cases",
                StartRangeFromZero = true

            };

            SplineSeries series = new SplineSeries()
            {
                ItemsSource = myDataCumulative.Data,
                XBindingPath = "Date",
                YBindingPath = "TotalCases",
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                ShowTooltip = true
            };


            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);

        }

        private void rollingSevenCaseOnClick(object sender, RoutedEventArgs e)
        {
            SfChart chart = new SfChart()
            {
                Margin = new Thickness(25, 25, 25, 25),
                Palette = ChartColorPalette.Metro,
                Header = "Rolling Cases of Covid-19 Tally Last 7 Days"
            };

            chart.PrimaryAxis = new CategoryAxis()
            {
                Header = "Date",
                PlotOffset = 10,
                LabelRotationAngle = -60
            };

            chart.SecondaryAxis = new NumericalAxis()
            {
                Minimum = 0,
                Header = "Cases",
                StartRangeFromZero = true

            };

            SplineSeries series = new SplineSeries()
            {
                ItemsSource = myData7.Data,
                XBindingPath = "Date",
                YBindingPath = "ConfirmedCases",
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                ShowTooltip = true
            };
            ChartAdornmentInfo adornmentInfo = new ChartAdornmentInfo()
            {
                ShowLabel = true,
                LabelPosition = AdornmentsLabelPosition.Auto,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(1),
                FontStyle = FontStyles.Italic,
                SegmentLabelContent = LabelContent.YValue,
                FontFamily = new FontFamily("Calibri"),
                FontSize = 11
            };

            series.AdornmentsInfo = adornmentInfo;

            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);
        }

        private void rollingThirtyCaseOnClick(object sender, RoutedEventArgs e)
        {
            SfChart chart = new SfChart()
            {
                Margin = new Thickness(25, 25, 25, 25),
                Palette = ChartColorPalette.Metro,
                Header = "Rolling Cases of Covid-19 Tally Last 30 Days"
            };

            chart.PrimaryAxis = new CategoryAxis()
            {
                Header = "Date",
                PlotOffset = 10,
                LabelRotationAngle = -60
            };

            chart.SecondaryAxis = new NumericalAxis()
            {
                Minimum = 0,
                Header = "Cases",
                StartRangeFromZero = true

            };

            SplineSeries series = new SplineSeries()
            {
                ItemsSource = myData30.Data,
                XBindingPath = "Date",
                YBindingPath = "ConfirmedCases",
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(00, 00, 02),
                ShowTooltip = true
            };
            ChartAdornmentInfo adornmentInfo = new ChartAdornmentInfo()
            {
                ShowLabel = true,
                LabelPosition = AdornmentsLabelPosition.Auto,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(1),
                FontStyle = FontStyles.Italic,
                SegmentLabelContent = LabelContent.YValue,
                FontFamily = new FontFamily("Calibri"),
                FontSize = 11
            };

            series.AdornmentsInfo = adornmentInfo;

            if (myGrid.Children.Count > 1)
            {
                myGrid.Children.RemoveAt(1);
            }

            chart.Series.Add(series);
            myGrid.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 0);
        }

        static public void GetData(string url , string outputPath)
        {
            string html = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine("D:\\PICTURES", outputPath)))
            {
                outputFile.WriteLine(html);
            }

        }
    }



}


