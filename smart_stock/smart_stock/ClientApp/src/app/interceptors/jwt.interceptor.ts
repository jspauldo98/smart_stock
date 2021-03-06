import { Injectable, Inject } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor} from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoginService } from '../services/login.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    basePath: string;
    
    constructor(private loginService: LoginService, @Inject('BASE_URL') baseUrl: string){this.basePath = baseUrl}

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        const accessToken = localStorage.getItem('access_token');
        //const isApiUrl = request.url.startsWith(this.basePath);
        if (accessToken) {
            request = request.clone({ setHeaders: {Authorization: `Bearer ${accessToken}`},
            });
        }
        return next.handle(request);
    }
    
}