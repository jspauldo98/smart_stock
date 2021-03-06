import { LoginService } from './login.service';

export function appInitializer(loginService: LoginService) {
    return () => 
        new Promise((resolve) => {
            loginService.refreshToken().subscribe().add(resolve);
        });
}