import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  public forecasts?: WeatherForecast[];

  private fileName: string = '';
  constructor(private http: HttpClient) {
    http.get<WeatherForecast[]>('/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => console.error(error)
    );
  }

  title = 'angularapp';

  public async processFile(imageInput: any) {
    const file: Blob = imageInput.files[0];
    this.fileName = imageInput.files[0].name;
    if (file) {
      const formData = new FormData();
      formData.append('file', file, this.fileName);
      this.http.post('/weatherforecast', formData).subscribe(
        (resolve) => {
          console.log(resolve);
        },
        (error) => {
          console.log(error);
        }
      );
    }
  }

  private async fileToByteArray(file: File): Promise<Uint8Array> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        if (reader.result instanceof ArrayBuffer) {
          resolve(new Uint8Array(reader.result));
        } else {
          reject('Failed to read file as an ArrayBuffer');
        }
      };
      reader.readAsArrayBuffer(file);
    });
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
