Token Authentication Example Code
===
```
> curl -I 'http://token-authentication-example-code.isprime.com/test-720p.mp4'
HTTP/1.1 403 Forbidden
[snip]
> php md5-authentication.php http://token-authentication-example-code.isprime.com/test-720p.mp4
http://token-authentication-example-code.isprime.com/test-720p.mp4?expires=1425631400&token=5a8a5f4576ce8878c27747d844321a5a
> python md5-authentication.py http://token-authentication-example-code.isprime.com/test-720p.mp4
http://token-authentication-example-code.isprime.com/test-720p.mp4?expires=1425631400&token=5a8a5f4576ce8878c27747d844321a5a
> curl -I 'http://token-authentication-example-code.isprime.com/test-720p.mp4?expires=1425631400&token=5a8a5f4576ce8878c27747d844321a5a'
HTTP/1.1 200 OK
[snip]
> 
```
