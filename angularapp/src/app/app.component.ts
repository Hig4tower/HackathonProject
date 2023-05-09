import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  public forecasts?: WeatherExtension[];
  private fileName: string = '';

  constructor(private http: HttpClient) {
    this.getData();
  }

  public getData() {
    this.http.get<WeatherForecast[]>('/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result as WeatherExtension[];
      },
      (error) => console.error(error)
    );
  }

  public async processFile(imageInput: any) {
    const file: Blob = imageInput.files[0];
    this.fileName = imageInput.files[0].name;
    if (file) {
      const formData = new FormData();
      formData.append('file', file, this.fileName);
      this.http.post<WeatherExtension>('/weatherforecast', formData).subscribe(
        (resolve) => {
          this.forecasts?.push(resolve);
        },
        (error) => {
          console.log(error);
        }
      );
    }
  }

  public deleteEntry(forecast: WeatherExtension) {
    this.forecasts = this.forecasts?.filter(x => x.summary !== forecast.summary);
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface WeatherExtension extends WeatherForecast {
  dayOrNight: string;
  humidity: number;
  precipitationProbability: number;
  windK: number;
  windM: number;
  windDir: string;
}
